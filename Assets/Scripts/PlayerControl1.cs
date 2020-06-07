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

    public GameObject StageManager;
    ItemManager StarManager;
    bool walk = false;
    bool StartMove;

    Vector3 KeepPos;
    bool HitBoxCol = false;

    bool StopNow = true;
    float HitNum = 0f;
    bool Jp;
    //反転判定
    bool HantenFg;
    //jpフラグをonにするか
    bool HitJpCheck;
    bool HitWall;
    GameObject penguinChild;
    GameObject SakaBlock;
    //坂道落下中
    bool DownFg;
    Vector2 KeepVec;

    // Start is called before the first frame update
    void Start()
    {
        penguinChild = transform.GetChild(0).gameObject;
        peguin = penguinChild.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        StarManager = StageManager.GetComponent<ItemManager>();
      
        StartMove = false;
        KeepPos = transform.position;
        Jp = false;
        HantenFg = false;
        HitJpCheck = false;
        HitWall = false;
        dir = 1;
        rb.Sleep();
        DownFg = false;
    }

    void FixedUpdate()
    {
        if (walk)
        {
            rb.velocity = new Vector2(transform.localScale.x * Time.deltaTime * playerspeed, rb.velocity.y);
                    }
        if (Jp)
        {
            Debug.Log("飛んだ");
            if (HitNum < 2f)
            {
                rb.velocity = new Vector2(0,3.7f);
            }
            else if(HitNum<3f)
            {
                rb.velocity = new Vector2(0f,5.5f);
            }
            
            HitNum = 0f;
            Jp = false;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject == SakaBlock)
        {
            walk = true;
            DownFg = false;
            HitNum = 0;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        //坂道下っているか判定
        if (collision.gameObject.tag == "block")
        {
            foreach (ContactPoint2D point in collision.contacts)
            {
                Vector2 kudari = new Vector2(point.point.x, point.point.y);
                Vector2 kudari2 = kudari;

                kudari.x += 3f;
                kudari2.x -= 3f;
                Vector2 saka = CheckKudari(collision.transform, kudari, kudari2);
                if (Mathf.Abs(saka.x) > 0.5f)
                {
                    if (Mathf.Abs(saka.y) > 0.5f)
                    {
                        SakaBlock = collision.gameObject;
                        walk = false;
                        DownFg = true;
                    }
                }
            }
        }
    }

    //ジャンプ判定用
    void OnTriggerEnter2D(Collider2D col)
    {
        if (!HantenFg)
        {
            if (col.gameObject.tag == "block")
            {
                //ジャンプ力調整のため
                HitNum+=1f;

                HitJpCheck = true;
                Vector2 watasu = col.ClosestPoint(this.transform.position);
                Vector2 watasu2 = col.ClosestPoint(this.transform.position);

                watasu2.x += 6f;
                watasu.x -= 6f;
                float back = CheckCrossPoint(col.transform, watasu, watasu2);
                //坂だった場合反転
                if (Mathf.Abs(back) > 0f)
                {
                    HitJpCheck = false;
                    HantenFg = true;
                    HitNum = 0;
                }
                    
            }

        }
        if (col.gameObject.tag == "Wall")
        {
            HitWall = true;
        }
        if (StarManager.GetAllFlg())
        {
            if (col.gameObject.tag == "Goal")
            {
                peguin.SetBool("Goal_1", true);
                playerspeed = 0;
            }
        }
    }


    // Update is called once per frame
    public void Update()
    {
        if (StopNow)
        {
            transform.position = KeepPos;
        }
        else
        {
            if (HitJpCheck)
            {
                Jp = true;
                HitJpCheck = false;
            }

            if (HitWall||HantenFg)
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
            }
        }
    }

   
    public void StopWalk()
    {
        if (StartMove)
        {
            walk = false;
      
            KeepPos = transform.position;

            StopNow = true;
            KeepVec = rb.velocity;
            rb.Sleep();
        }
    }

    public void StartWalk()
    {
        if (StartMove)
        {
            if (!DownFg)
            {
                walk = true;
            }

            rb.WakeUp();

            StopNow = false;
            rb.WakeUp();
            rb.velocity = KeepVec;

        }
    }

    public void LetsStart()
    {
        rb.WakeUp();
        StartMove = true;
        walk = true;
        StopNow = false;
        transform.position = KeepPos;
    }

    public void HitChild()
    {
        HantenFg = true;
    }

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
        
        float nearpoint;
        Vector2 TouchVec = new Vector2(0, 0);
        //右向き
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
       
        //交差した頂点を保存しておいて　Playerのポジションと比較をして近いほうの傾きを利用して判断する
        for (int i = 1; i < ParentIce.childCount-1; i++)
        {
            I_pos = ParentIce.GetChild(i).position;
            I_pos2 = ParentIce.GetChild(i + 1).position;
            Pos1 = new Vector2(I_pos.x, I_pos.y);
            Pos2 = new Vector2(I_pos2.x, I_pos2.y);
            if (LineSegmentsIntersection(P_pos, Hitpos, Pos1, Pos2, out CrossPoint))
            {
                if (dir > 0f)
                {
                    if (nearpoint > CrossPoint.x)
                    {
                        Vector2 seikou = new Vector2(Pos1.x - Pos2.x, Pos1.y - Pos2.y);
                        nearpoint = CrossPoint.x;
                        TouchVec = seikou;
                    }
                }
                else
                {
                    if (nearpoint < CrossPoint.x)
                    {
                        Vector2 seikou = new Vector2(Pos1.x - Pos2.x, Pos1.y - Pos2.y);
                        nearpoint = CrossPoint.x;
                        TouchVec = seikou;
                    }
                }
                
            }
         
            if (i == ParentIce.childCount - 2)
            {
                I_pos = ParentIce.GetChild(1).position;
                Pos1 = new Vector2(I_pos.x, I_pos.y);
             
                if (LineSegmentsIntersection(P_pos, Hitpos, Pos1, Pos2, out CrossPoint))
                {

                    if (dir > 0)
                    {
                        if (nearpoint > CrossPoint.x)
                        {
                            Vector2 seikou = new Vector2(Pos1.x - Pos2.x, Pos1.y - Pos2.y);
                            nearpoint = CrossPoint.x;
                            TouchVec = seikou;
                        }
                    }
                    else
                    {
                        if (nearpoint < CrossPoint.x)
                        {
                            Vector2 seikou = new Vector2(Pos1.x - Pos2.x, Pos1.y - Pos2.y);
                            nearpoint = CrossPoint.x;
                            TouchVec = seikou;
                        }
                    }
                }
            }

        }
        if (TouchVec.y > 0f)
        {
            TouchVec = -TouchVec;
        }
        return TouchVec.x;
    }

    Vector2 CheckKudari(Transform ParentIce, Vector2 Hitpos, Vector2 P_pos)
    {
        Vector2 CrossPoint;

        float nearpoint;
        Vector2 TouchVec = new Vector2(0, 0);
        //右向き
        if (dir < 0f)
        {
            nearpoint = 1000f;
        }
        else
        {
            nearpoint = -1000f;
        }

        Vector2 Pos1, Pos2;
        Vector3 I_pos, I_pos2;

        //交差した頂点を保存しておいて　Playerのポジションと比較をして近いほうの傾きを利用して判断する
        for (int i = 1; i < ParentIce.childCount - 1; i++)
        {
            I_pos = ParentIce.GetChild(i).position;
            I_pos2 = ParentIce.GetChild(i + 1).position;
            Pos1 = new Vector2(I_pos.x, I_pos.y);
            Pos2 = new Vector2(I_pos2.x, I_pos2.y);
            if (LineSegmentsIntersection(P_pos, Hitpos, Pos1, Pos2, out CrossPoint))
            {
                if (dir > 0f)
                {
                    if (nearpoint < CrossPoint.x)
                    {
                        Vector2 seikou = new Vector2(Pos1.x - Pos2.x, Pos1.y - Pos2.y);
                        nearpoint = CrossPoint.x;
                        TouchVec = seikou;
                    }
                }
                else
                {
                    if (nearpoint > CrossPoint.x)
                    {
                        Vector2 seikou = new Vector2(Pos1.x - Pos2.x, Pos1.y - Pos2.y);
                        nearpoint = CrossPoint.x;
                        TouchVec = seikou;
                    }
                }

            }

            if (i == ParentIce.childCount - 2)
            {
                I_pos = ParentIce.GetChild(1).position;
                Pos1 = new Vector2(I_pos.x, I_pos.y);

                if (LineSegmentsIntersection(P_pos, Hitpos, Pos1, Pos2, out CrossPoint))
                {

                    if (dir > 0)
                    {
                        if (nearpoint < CrossPoint.x)
                        {
                            Vector2 seikou = new Vector2(Pos1.x - Pos2.x, Pos1.y - Pos2.y);
                            nearpoint = CrossPoint.x;
                            TouchVec = seikou;
                        }
                    }
                    else
                    {
                        if (nearpoint > CrossPoint.x)
                        {
                            Vector2 seikou = new Vector2(Pos1.x - Pos2.x, Pos1.y - Pos2.y);
                            nearpoint = CrossPoint.x;
                            TouchVec = seikou;
                        }
                    }
                }
            }

        }
        if (TouchVec.y > 0f)
        {
            TouchVec = -TouchVec;
        }
        return TouchVec;
    }
}
