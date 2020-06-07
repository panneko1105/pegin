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

        if (!SpownMode)
        {
            //----------------------------------------------
            //  氷生成モード (Xボタン)
            //----------------------------------------------
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 2"))
            {
                GameObject obj2 = (GameObject)Resources.Load("maskBox");
                Vector3 Setpos2 = maincamera.transform.position;
                Setpos2.z = 1f;
                Setpos2.y += 1f;
                obj2 = Instantiate(obj2, Setpos2, Quaternion.identity);
                obj2.transform.SetParent(this.transform);

                GameObject obj = (GameObject)Resources.Load("flame");
                Vector3 Setpos = maincamera.transform.position;
                Setpos.z = 1f;
                obj = Instantiate(obj, Setpos, Quaternion.identity);
                obj.transform.SetParent(this.transform);
                obj.tag = "flame";
                
                SpownMode = true;

                StopMono.enabled = true;
                WalkCon.StopWalk();
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
                Transform KeepMask = null;
                foreach (Transform child in this.transform)
                {   
                    if (child.tag == "Mask")
                    {
                        //マスクボックスを保存
                        KeepMask = child;
                    }
                    else if(child.tag == "flame")
                    {
                        var sc = child.GetComponent<FlameMove>();
                        if (sc.Mabiki(Player.transform.position))
                        {
                            //SoundManager.Instance.StopSe();
                            // SE再生
                            SoundManager.Instance.PlaySe("凍る・コチーン");

                            var col = KeepMask.GetComponent<PolygonCollider2D>();
                            col.isTrigger = false;

                            sc.CreateIce();

                            SpownMode = false;
                            if (OnceMove)
                            {
                                WalkCon.LetsStart();
                                OnceMove = false;
                            }
                            else
                            {
                                // 歩き出すよ
                                WalkCon.StartWalk();
                            }
                            StopMono.enabled = false;
                            Destroy(KeepMask.gameObject);

                            DeleteChild();
                            PushNum++;
                            IceNum++;
                            if (PushNum == DelNum - 1) 
                            {
                                transform.GetChild(0).gameObject.AddComponent<Tenmetu>();
                            }
                        }
                     
                    }
                   
                }
                SoundManager.Instance.StopSeEX("near_a_brook");
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
            Instantiate(obj, child.transform.position, Quaternion.identity);

            Destroy(child.gameObject);
            transform.GetChild(1).gameObject.AddComponent<Tenmetu>();
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