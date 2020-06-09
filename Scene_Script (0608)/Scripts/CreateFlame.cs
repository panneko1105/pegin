using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public class CreateFlame : MonoBehaviour, IUpdatable
{
    public Camera maincamera;
    public int DelNum;
    private bool SpownMode = false;         //trueで出ている状態

    public GameObject Player;
    public GameObject Camera;
    PlayerControl1 WalkCon;
    Post StopMono;
    bool OnceMove;
    int PushNum = 0;
    int IceNum = 0;
    //点滅スクリプト保持
    Tenmetu ten = null;

    void OnEnable()
    {
        UpdateManager.AddUpdatable(this);
        WalkCon = Player.GetComponent<PlayerControl1>();
        OnceMove = true;
        StopMono = Camera.GetComponent<Post>();
    }

    void OnDisable()
    {
        UpdateManager.RemoveUpdatable(this);
    }

    // Use this for initialization
    public void UpdateMe()
    {
        // 操作不可
        if (StageManager.Instance.GetFlg() != StageFlg.NOMAL)
        {
            return;
        }

        //氷生成中に追加で生成させないため
        if (!SpownMode)
        {
            //----------------------------------------------
            //  氷生成モード (Xボタン)
            //----------------------------------------------
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 2"))

            {

                //マスク処理用のCube生成-----------------------------------------------------------
                GameObject obj2 = (GameObject)Resources.Load("maskBox");
                Vector3 Setpos2 = maincamera.transform.position;
                Setpos2.z = 1f;
                Setpos2.y += 1f;
                obj2 = Instantiate(obj2, Setpos2, Quaternion.identity);
                obj2.transform.SetParent(this.transform);
                //---------------------------------------------------------------------------------
                
                //flameの生成-----------------------------------------------------------------------
                GameObject obj = (GameObject)Resources.Load("flame");
                Vector3 Setpos = maincamera.transform.position;
                Setpos.z = 1f;
                obj = Instantiate(obj, Setpos, Quaternion.identity);
                obj.transform.SetParent(this.transform);
                obj.tag = "flame";
                //---------------------------------------------------------------------------------
                
                //生成可能状態に
                SpownMode = true;
                //画面演出ON
                StopMono.enabled = true;
                //プレイヤーの歩行ストップ
                WalkCon.StopWalk();

                //点滅on
                if (ten != null)
                {
                    ten.enabled = true;
                } 
            }
        }
        else
        {
            // ポーズ解除と同時に氷が落ちるのを防止
            if (PauseManager.Instance.GetPauseEndCnt() < 1)
            {
                return;
            }

            //----------------------------------------------
            //  氷生成モード (Aボタン)
            //----------------------------------------------
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
            {
                //マスクboxのキャッシュ削除
                Transform KeepMask = null;
                foreach (Transform child in this.transform)
                {   
                    if (child.tag == "Mask")
                    {
                        //マスクボックスを保存
                        KeepMask = child;
                    }
                    else if(child.tag=="flame")
                    {
                        var sc = child.GetComponent<FlameMove>();
                        //生成可能か判定
                        if (sc.Mabiki(Player.transform.position))
                        {
                            //SoundManager.Instance.StopSe();
                            // SE再生
                            SoundManager.Instance.PlaySe("凍る・コチーン");

                            //くりぬきのためトリガーtrueに
                            var col = KeepMask.GetComponent<PolygonCollider2D>();
                            col.isTrigger = false;
                            sc.CreateIce();
                            //生成可能状態に
                            SpownMode = false;
                            //最初の動きだし用
                            if (OnceMove)
                            {
                                WalkCon.LetsStart();
                                OnceMove = false;
                            }
                            //プレイヤーの移動開始
                            else
                            {
                                WalkCon.StartWalk();
                            }
                            //画面演出off
                            StopMono.enabled = false;
                            Destroy(KeepMask.gameObject);
                            //生成されたので消す子供がいないかチェック
                            DeleteChild();
                            PushNum++;
                            IceNum++;
                            //点滅スクリプトを最初だけつけるため
                            if (PushNum == DelNum - 1) 
                            {
                                ten = transform.GetChild(0).gameObject.AddComponent<Tenmetu>();
                            }
                            ten.enabled = false;
                        }
                     
                    }
                   
                }
                // 水位上下のSE停止
                SoundManager.Instance.StopSeEX("near_a_brook");
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                //プレイヤーの移動開始
                if (!OnceMove)
                {
                    WalkCon.StartWalk();
                }

                foreach (Transform child in this.transform)
                {
                    if (child.tag == "Mask")
                    {
                        Destroy(child.gameObject);
                    }
                    if (child.tag == "flame")
                    {
                        Destroy(child.gameObject);
                    }
                }
                //生成可能状態に
                SpownMode = false;
                //画面演出off
                StopMono.enabled = false;
                ten.Cancel();
                //点滅off
                ten.enabled = false;
            }
        }
    }

    public void DeleteChild()
    {
        if (this.transform.childCount > DelNum) 
        {
            var child = transform.GetChild(0);
            //エフェクト発生
            GameObject obj = (GameObject)Resources.Load("icebreak");
            Vector3 EfectPos = child.transform.position;
            EfectPos.y -= 1.0f;
            Instantiate(obj, EfectPos, Quaternion.identity);

            Destroy(child.gameObject);
            ten = transform.GetChild(1).gameObject.AddComponent<Tenmetu>();
            IceNum--;
        }
    }

    public int GetDelNum()
    {
        return DelNum;
    }
    public int GetIceNum()
    {
        return IceNum;
    }
}