using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public class PlayerControl1 : MonoBehaviour/*,IUpdatable*/
{

    public float playerspeed;
    private Rigidbody2D rb;
    Animator peguin;
    private float dir = 1f;

    public GameObject StaManager;
    ItemManager StarManager;
    bool walk = false;
    bool StartMove;

    Vector3 KeepPos;
    bool HitBoxCol = false;
    //止まっているか
    bool StopNow = true;
    //ジャンプ力調整用
    int HitNum = 0;
    bool Jp;
    //反転判定
    bool HantenFg;
    //jpフラグをonにするか
    bool HitJpCheck;
    //壁に当たったかどうか
    bool HitWall;
    GameObject penguinChild;
    GameObject SakaBlock;
    //坂道落下中
    bool DownFg;
    //停止時のベクトルを保持しておくため
    Vector2 KeepVec;
    //各段階のジャンプ力------------------
    public Vector2 Jp_Fase1;
    public Vector2 Jp_Fase2;
    public Vector2 Jp_Fase3;
    //------------------------------------
    //ジャンプモーションなどを一度だけ行うため
    bool OnceJpFg = false;
    bool KudariCancelFg = false;

    // ラストステージ演出用
    bool isFianlEffect = false;

    public bool GetWalking()
    {
        return walk;
    }

    // Start is called before the first frame update
    void Start()
    {
        penguinChild = transform.GetChild(0).gameObject;
        peguin = penguinChild.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        StarManager = StaManager.GetComponent<ItemManager>();
      
        StartMove = false;
        KeepPos = transform.position;
        KeepVec = rb.velocity;
        Jp = false;
        HantenFg = false;
        HitJpCheck = false;
        HitWall = false;
        dir = 1;
        rb.isKinematic = true;
        DownFg = false;
        SakaBlock = null;
    }

    void FixedUpdate()
    {
        if (walk)
        {
            //歩いている場合
            rb.velocity = new Vector2(transform.localScale.x * Time.deltaTime * playerspeed, rb.velocity.y);
        }
        if (Jp)
        {
            //ジャンプする場合

            //少し後退する
            Vector3 BackPos = transform.position;
            BackPos.x += -(dir * 0.05f);
            transform.position = BackPos;
         
            Vector2 Jp_Power;
            //向きによってベクトルを変えるため一時代入してベクトルを反転
            switch (HitNum)
            {
                case 1:
                    Jp_Power = Jp_Fase1;
                    Jp_Power.x *= dir;
                    rb.velocity = Jp_Power;
                    break;

                case 2:
                    Jp_Power = Jp_Fase2;
                    Jp_Power.x *= dir;
                    rb.velocity = Jp_Power;
                    break;

                case 3:
                    Jp_Power = Jp_Fase3;
                    Jp_Power.x *= dir;
                    rb.velocity = Jp_Power;
                    break;
            }
          
            HitNum = 0;
            Jp = false;
            walk = false;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        //現在乗っていた坂から降りた場合
        if (collision.gameObject == SakaBlock)
        {
            //坂道から降りた
            walk = true;
            DownFg = false;
            HitNum = 0;
            SakaBlock = null;

            // 滑りSE停止
            //坂アニメーション終了
            peguin.SetBool("SaKa", false);
            SoundManager.Instance.StopSeEX("cute-sad1_EX");
            peguin.SetBool("Walk", true); // 歩きアニメーション開始
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        //坂道下っているか判定
        if (collision.gameObject.tag == "block")
        {
            //坂に触れていない場合
            if (SakaBlock == null)
            {
                //足音再開！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
                SoundManager.Instance.PlaySeEX("Step_EX");
                //ジャンプモーション終了
                OnceJpFg = false;
                walk = true;
                Debug.Log("ジャンプモーション終了");

                //エフェクト発生
                GameObject obj2 = (GameObject)Resources.Load("CFX3_Hit_SmokePuff");
                Vector3 efePos = transform.position;
                efePos.y -= 0.6f;
                Instantiate(obj2, efePos, Quaternion.identity);

            //} ほんとはIF文ここまで
                foreach (ContactPoint2D point in collision.contacts)
                {
                    Vector2 kudari = new Vector2(point.point.x, point.point.y);
                    Vector2 kudari2 = kudari;

                    kudari.y += 3f;
                    kudari2.y -= 3f;
                    float baxk = CheckKudari2(collision.transform, kudari, kudari2);
                    float gori = Mathf.Abs(baxk) - 45f;
                    if (Mathf.Abs(gori)<15.5f)
                    {

                    }
                    else if (baxk > 20f)
                    {
                        Debug.Log("坂滑り" + baxk);
                        //坂道下り始め
                        SakaBlock = collision.gameObject;
                        walk = false;
                        DownFg = true;
                        peguin.SetBool("SaKa", true);//坂アニメーション開始
                        peguin.SetBool("Walk", false);
                        // 滑りSE開始
                     
                        SoundManager.Instance.PlaySeEX("cute-sad1_EX");
                        // 歩きアニメーション終了
                        peguin.SetBool("Walk", false); 
                        SoundManager.Instance.StopSeEX("Step_EX");
                    }
                }
            }
        }
    }

    //ジャンプ判定用
    void OnTriggerEnter2D(Collider2D col)
    {
        //子供の反転用コライダーに当たっていない場合
        if (!HantenFg)
        {
            if (col.gameObject.tag == "block")
            {
                //落下中に触れた場合飛ばなくするため
                if (Mathf.Abs(rb.velocity.y) < 2.0f)
                {
                    //ジャンプ力調整のため
                    HitNum++;
                }
                //この段階ではまだ飛ばない
                HitJpCheck = true;
                //接触点の傾斜を判定----------------------------------------------------------------
                Vector2 watasu = col.ClosestPoint(this.transform.position);
                Vector2 watasu2 = col.ClosestPoint(this.transform.position);
                watasu2.y += 6f;
                watasu.y -= 6f;
                float back = CheckKudari2(col.transform, watasu, watasu2);//CheckCrossPoint(col.transform, watasu, watasu2);
                //坂だった場合反転
                if (back > 45f)
                {
                    Debug.Log("坂道です" + back);
                    HitJpCheck = false;
                    HantenFg = true;
                    HitNum = 0;
                }
                //------------------------------------------------------------------------------
                    
            }

        }
        if (col.gameObject.tag == "Wall")
        {
            HitWall = true;
        }
        //星をすべて集めてゴールに触れた場合クリア
        if (StarManager.GetAllFlg())
        {
            if (col.gameObject.tag == "Goal")
            {
                Debug.Log("End");
                peguin.SetBool("Goal_1", true);
                StartCoroutine(StageManager.Instance.GoalEvent());
                playerspeed = 0;
            }
        }
    }


    // Update is called once per frame
    public void Update()
    {
        if (StopNow)
        {
            //なぜかvelcityがSleepしないのでposを保存
            transform.position = KeepPos;
        }
        else
        {
            if (HitJpCheck)
            {
                Jp = true;
                Debug.Log("????");
                HitJpCheck = false;
                if (!OnceJpFg)
                {
                    //ジャンプ音入れて足音STOP！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
                    // SE
                    SoundManager.Instance.StopSeEX("Step_EX");
                    SoundManager.Instance.PlaySeEX("かわいく跳ねる・ジャンプ03");
                    OnceJpFg = true;
                }
            }

            if (HitWall||HantenFg)
            {
                //坂下り中は反転しない
                if (!DownFg)
                {
                    //反転処理
                    Vector3 temp = gameObject.transform.localScale;

                    //localScale.xに-1をかける

                    temp.x *= -1f;

                    //結果を戻す

                    gameObject.transform.localScale = temp;
                    dir *= -1;

                    HitBoxCol = true;
                    Jp = false;
                    HitWall = false;
                    HantenFg = false;
                    HitNum = 0;
                }
                else
                {
                    HantenFg = false;
                }
            }
        }

        // 最終面
        if (StageManager.Instance.GetFinalFlag())
        {
            if (!isFianlEffect)
            {
                //Debug.Log(this.transform.position.x);
                // 画面端
                if (this.transform.position.x > 17.5f)
                {
                    // 全氷消す
                    Debug.Log("Fianl：全氷消去");
                    GameObject obj = GameObject.Find("icemanager");
                    CreateFlame createFlame = obj.GetComponent<CreateFlame>();
                    createFlame.DeleteAllChildren();
                    // 溶けSE
                    SoundManager.Instance.PlaySeEX("溶ける音CESA");

                    isFianlEffect = true;
                }
            }
        }
    }

   
    public void StopWalk()
    {
        //一番最初以外の場合
        if (StartMove)
        {
            walk = false;
      
            KeepPos = transform.position;
            SoundManager.Instance.StopSeEX("Step_EX");
            peguin.SetBool("Walk", false);
            StopNow = true;
            KeepVec = rb.velocity;
            rb.isKinematic = true;
        }
      
    }

    public void StartWalk()
    {
        //一番最初以外の場合
        if (StartMove)
        {
            //坂を滑っていなかったら
            if (!DownFg)
            {
                walk = true;
                peguin.SetBool("Walk", true);
            }
            //物理演算OFF
            rb.isKinematic = false;
            StopNow = false;
            rb.WakeUp();
            rb.velocity = KeepVec;

            // SE再生
            SoundManager.Instance.PlaySeEX("Step_EX");
        }
    }

    public void LetsStart()
    {
        // SE再生
        SoundManager.Instance.PlaySeEX("Step_EX");

        rb.isKinematic = false;
        StartMove = true;
        walk = true;
        peguin.SetBool("Walk", true);
        StopNow = false;
        transform.position = KeepPos;
        rb.velocity = KeepVec;
    }

    public void HitChild()
    {
        HantenFg = true;
    }
    //ベクトルの交差点を求める
     static bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
    {
        intersection = Vector2.zero;

        var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0.0f)
        {
            return false;
        }

        var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        intersection.x = p1.x + u * (p2.x - p1.x);
        intersection.y = p1.y + u * (p2.y - p1.y);

        return true;
    }

    //坂道で反転するため
    float CheckCrossPoint(Transform ParentIce, Vector2 Hitpos,Vector2 P_pos)
    {
        Vector2 CrossPoint;
        //交わったベクトルで一番自信に近い点を採用するため
        float nearpoint;
        Vector2 TouchVec = new Vector2(0, 0);
        //向きによって必ず交点が代入されるように
        if (dir > 0f)
        {
            nearpoint = 1000f;
        }
        else
        {
            nearpoint = -1000f;
        }

        Vector2 Pos1, Pos2;
        Vector3 I_pos, I_pos2;

        float S_Angle = 0f;
        //交差した頂点を保存しておいて　Playerのポジションと比較をして近いほうの傾きを利用して判断する
        for (int i = 1; i < ParentIce.childCount-1; i++)
        {
            //各頂点のワールド座標を取得---------------------------------------------------------
            I_pos = ParentIce.GetChild(i).position;
            I_pos2 = ParentIce.GetChild(i + 1).position;
            Pos1 = new Vector2(I_pos.x, I_pos.y);
            Pos2 = new Vector2(I_pos2.x, I_pos2.y);
            //-----------------------------------------------------------------------------------
            //交差してるかチェック
            if (LineSegmentsIntersection(P_pos, Hitpos, Pos1, Pos2, out CrossPoint))
            {
                //交差した点を比較して近いほうのベクトルと傾斜を保存
                if (dir > 0f)
                {
                    if (nearpoint > CrossPoint.x)
                    {
                        Vector2 seikou = new Vector2(Pos1.x - Pos2.x, Pos1.y - Pos2.y);
                        if (Pos1.y < Pos2.y)
                        {
                            S_Angle = GetAngle(Pos1, Pos2);
                        }
                        else
                        {
                            S_Angle = GetAngle(Pos2, Pos1);
                        }

                        nearpoint = CrossPoint.x;
                        TouchVec = seikou;
                    }
                }
                else
                {
                    //交差した点を比較して近いほうのベクトルと傾斜を保存
                    if (nearpoint < CrossPoint.x)
                    {
                        Vector2 seikou = new Vector2(Pos1.x - Pos2.x, Pos1.y - Pos2.y);
                        if (Pos1.y < Pos2.y)
                        {
                            S_Angle = GetAngle(Pos1, Pos2);
                        }
                        else
                        {
                            S_Angle = GetAngle(Pos2, Pos1);
                        }

                        nearpoint = CrossPoint.x;
                        TouchVec = seikou;
                    }
                }
                
            }
         //最後の頂点と最初の頂点を結んだベクトルを作るため
            if (i == ParentIce.childCount - 2)
            {
                I_pos = ParentIce.GetChild(1).position;
                Pos1 = new Vector2(I_pos.x, I_pos.y);
             
                if (LineSegmentsIntersection(P_pos, Hitpos, Pos1, Pos2, out CrossPoint))
                {
                    //交差した点を比較して近いほうのベクトルと傾斜を保存
                    if (dir > 0)
                    {
                        if (nearpoint > CrossPoint.x)
                        {
                            Vector2 seikou = new Vector2(Pos1.x - Pos2.x, Pos1.y - Pos2.y);
                            if (Pos1.y < Pos2.y)
                            {
                                S_Angle = GetAngle(Pos1, Pos2);
                            }
                            else
                            {
                                S_Angle = GetAngle(Pos2, Pos1);
                            }
                            nearpoint = CrossPoint.x;
                            TouchVec = seikou;
                        }
                    }
                    else
                    {
                        if (nearpoint < CrossPoint.x)
                        {
                            Vector2 seikou = new Vector2(Pos1.x - Pos2.x, Pos1.y - Pos2.y);

                            if (Pos1.y < Pos2.y)
                            {
                                S_Angle = GetAngle(Pos1, Pos2);
                            }
                            else
                            {
                                S_Angle = GetAngle(Pos2, Pos1);
                            }

                            nearpoint = CrossPoint.x;
                            TouchVec = seikou;
                        }
                    }
                }
            }

        }
        //９０度以上の場合
        if (S_Angle > 90f)
        {
            S_Angle = 180f - S_Angle;
        }
        //直角に近似のものは坂の判定から外すため-----------------------------------------
        if (S_Angle > 85f)
        {
            S_Angle = 0f;
        }
        if (Mathf.Abs(TouchVec.x) < 0.3f || Mathf.Abs(TouchVec.y) < 0.3f)
        {
            S_Angle = 0;
        }//-----------------------------------------------------------------------------
        
        return S_Angle;
    }
    //上の関数の下りの坂道チェック用に書き換えたもの　ほぼ同じ

    float CheckKudari2(Transform ParentIce, Vector2 Hitpos, Vector2 P_pos)
    {
        Vector2 CrossPoint;

        float nearpoint;
        Vector2 TouchVec = new Vector2(0, 0);

        nearpoint = -1000f;

        Vector2 Pos1, Pos2;
        Vector3 I_pos, I_pos2;

        float S_Angle = 0f;
        //交差した頂点を保存しておいて　Playerのポジションと比較をして近いほうの傾きを利用して判断する
        for (int i = 1; i < ParentIce.childCount - 1; i++)
        {
            I_pos = ParentIce.GetChild(i).position;
            I_pos2 = ParentIce.GetChild(i + 1).position;
            Pos1 = new Vector2(I_pos.x, I_pos.y);
            Pos2 = new Vector2(I_pos2.x, I_pos2.y);
            if (LineSegmentsIntersection(P_pos, Hitpos, Pos1, Pos2, out CrossPoint))
            {
                if (nearpoint < CrossPoint.y)
                {
                    Vector2 seikou = new Vector2(0, 0);

                    if (Pos1.y < Pos2.y)
                    {
                        S_Angle = GetAngle(Pos1, Pos2);
                        seikou = new Vector2(Pos1.x - Pos2.x, Pos1.y - Pos2.y);
                    }
                    else
                    {
                        S_Angle = GetAngle(Pos2, Pos1);
                        seikou = new Vector2(Pos2.x - Pos1.x, Pos2.y - Pos1.y);
                    }

                    nearpoint = CrossPoint.x;
                    TouchVec = seikou;
                }

            }

            if (i == ParentIce.childCount - 2)
            {
                I_pos = ParentIce.GetChild(1).position;
                Pos1 = new Vector2(I_pos.x, I_pos.y);

                if (LineSegmentsIntersection(P_pos, Hitpos, Pos1, Pos2, out CrossPoint))
                {


                    if (nearpoint < CrossPoint.y)
                    {
                        Vector2 seikou = new Vector2(0, 0);
                        if (Pos1.y < Pos2.y)
                        {
                            S_Angle = GetAngle(Pos1, Pos2);
                            seikou = new Vector2(Pos1.x - Pos2.x, Pos1.y - Pos2.y);
                        }
                        else
                        {
                            S_Angle = GetAngle(Pos2, Pos1);
                            seikou = new Vector2(Pos2.x - Pos1.x, Pos2.y - Pos1.y);
                        }
                        nearpoint = CrossPoint.x;
                        TouchVec = seikou;
                    }

                }
            }

        }
        if (S_Angle > 90f)
        {
            S_Angle = 180f - S_Angle;
        }

        if (S_Angle > 85f)
        {
            S_Angle = 0f;
        }
        if (Mathf.Abs(TouchVec.x) < 0.3f || Mathf.Abs(TouchVec.y) < 0.3f)
        {
            S_Angle = 0;
        }
        //正の向きのとき

        return S_Angle;
    }

    //２頂点の角度判定
    public float GetAngle(Vector2 p1, Vector2 p2)
    {
        float dx = p2.x - p1.x;
        float dy = p2.y - p1.y;
        float rad = Mathf.Atan2(dy, dx);
        return rad * Mathf.Rad2Deg;
    }
}
