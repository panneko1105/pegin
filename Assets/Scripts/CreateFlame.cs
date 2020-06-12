using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;
using UnityEngine.UI;

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

    //
    //[SerializeField] GameObject iceNumtext; //!< 氷制限数の表記
    //

    void OnEnable()
    {
        UpdateManager.AddUpdatable(this);
        WalkCon = Player.GetComponent<PlayerControl1>();
        OnceMove = true;
        StopMono = Camera.GetComponent<Post>();

        // 氷制限数の表記更新
        GameObject iceNumtext = GameObject.Find("Text_IceNum");
        Text t = iceNumtext.GetComponent<Text>();
        t.text = DelNum.ToString();
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
            //  氷生成モード (Aボタン)
            //----------------------------------------------
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown("joystick button 0"))
            {

                //マスク処理用のCube生成-----------------------------------------------------------
                GameObject obj2 = (GameObject)Resources.Load("maskBox");
                Vector3 Setpos2 = maincamera.transform.position;
                Setpos2.z = 1f;
                Setpos2.y += 2.5f;
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
        // 氷生成モード中
        else
        {
            // ポーズ解除と同時に氷が落ちるのを防止
            if (PauseManager.Instance.GetPauseEndCnt() < 1)
            {
                return;
            }
            //氷全削除！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
            if (Input.GetKeyDown(KeyCode.Q))
            {
                DeleteAllChildren();
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
                            // 水位上下のSE停止
                            SoundManager.Instance.StopSeEX("near_a_brook");
                            // SE再生
                            SoundManager.Instance.PlaySeEX("氷1");

                            //くりぬきのためトリガーtrueに
                            var col = KeepMask.GetComponent<PolygonCollider2D>();
                            col.isTrigger = false;
                            sc.CreateIce();
                            //生成可能状態に
                            Debug.Log("あああ");
                            SpownMode = false;
                            //最初の動きだし用
                            if (OnceMove)
                            {
                                SoundManager.Instance.PlaySeEX("Step_EX");
                                WalkCon.LetsStart();
                                OnceMove = false;
                            }
                            //プレイヤーの移動開始
                            else
                            {
                                //SoundManager.Instance.PlaySeEX("Step_EX");
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
                            if (PushNum == DelNum) 
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

            //----------------------------------------------
            //  氷生成キャンセル (Bボタン)
            //----------------------------------------------
            if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown("joystick button 1"))
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
        if (this.transform.childCount > DelNum + 1) 
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

            // 消えSE
            SoundManager.Instance.PlaySeEX("溶ける音CESA");
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

    void DeleteAllChildren()
    {
        foreach(Transform child in this.transform)
        {
            if (child.tag == "block")
            {
                
                //エフェクト発生
                GameObject obj = (GameObject)Resources.Load("icebreak");
                Vector3 EfectPos = child.transform.position;
                EfectPos.y -= 1.0f;
                Instantiate(obj, EfectPos, Quaternion.identity);

                Destroy(child.gameObject);
                IceNum = 0;
                PushNum = 0;
            }
        }

        // 消えSE
        SoundManager.Instance.PlaySeEX("溶ける音CESA");
    }
}