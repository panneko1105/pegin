using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.VFX;
using GokUtil.UpdateManager;


public class maskBoxMove : MonoBehaviour, IUpdatable
{
    public float move = 5.0f;
    [HideInInspector] public int flg = 0;
    [HideInInspector] public bool isRot = false;        //trueで傾いている
    private List<Vector3> v = new List<Vector3>();

    private int fase;

    PolygonCollider2D m_ObjectCollider;

    private GameObject _child;

    private DrawMesh dr;

    bool pushFlg = false;         //!< 入力フラグ
    int pushCnt = 0;              //!< 押し続け
    int cntAida = 4;
    int cntBigin = 10;
    bool seFlg = false;

    // Start is called before the first frame update
    void Start()
    {
        m_ObjectCollider = GetComponent<PolygonCollider2D>();
        fase = 4;

        dr = this.gameObject.GetComponent<DrawMesh>();

#if UNITY_EDITOR
        cntAida = 12;
        cntBigin = 40;
#else
        cntAida = 4;
        cntBigin = 10;
#endif
    }

    void OnEnable()
    {
        UpdateManager.AddUpdatable(this);
    }

    void OnDisable()
    {
        UpdateManager.RemoveUpdatable(this);
    }


    // Update is called once per frame
    public void UpdateMe()
    {
        // 操作不可
        if (StageManager.Instance.GetFlg() != StageFlg.NOMAL)
        {
            return;
        }

        //----------------------------------------------
        //  フレーム移動 (左スティック)
        //----------------------------------------------
        // L Stick
        float lsh = Input.GetAxis("L_Stick_H");
        float lsv = Input.GetAxis("L_Stick_V");
        // 十字キー
        float dph = Input.GetAxis("D_Pad_H");
        float dpv = Input.GetAxis("D_Pad_V");
        // 押してない判定
        if (lsv == 0 && dpv == 0 && lsh == 0 && dph == 0)
        {
            //Debug.Log("Yeah!");
            pushFlg = false;
            pushCnt = 0;
        }

        // ↑移動
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.position += new Vector3(0, 1f, 0);
        }
        if (lsv > 0.1f || dpv > 0.0f)
        {
            // 1回目 or 180回目以降+60ごと
            if ((!pushFlg || (pushCnt >= cntBigin && pushCnt % cntAida == 0)))
            {
                transform.position += new Vector3(0, 1f, 0);
                pushFlg = true;
            }
            pushCnt++;
        }
        // ↓移動
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position += new Vector3(0, -1f, 0);
        }
        if (lsv < -0.1f || dpv < -0.0f)
        {
            // 1回目 or 180回目以降+60ごと
            if ((!pushFlg || (pushCnt >= cntBigin && pushCnt % cntAida == 0)))
            {
                transform.position += new Vector3(0, -1f, 0);
                pushFlg = true;
            }
            pushCnt++;
        }
        // →に移動
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position += new Vector3(1f, 0, 0);
        }
        if (lsh > 0.1f || dph > 0.0f)
        {
            // 1回目 or 180回目以降+60ごと
            if ((!pushFlg || (pushCnt >= cntBigin && pushCnt % cntAida == 0)))
            {
                transform.position += new Vector3(1f, 0, 0);
                pushFlg = true;
            }
            pushCnt++;
        }
        // ←に移動
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position += new Vector3(-1f, 0, 0);
        }
        if (lsh < -0.1f || dph < -0.0f)
        {
            // 1回目 or 180回目以降+60ごと
            if ((!pushFlg || (pushCnt >= cntBigin && pushCnt % cntAida == 0)))
            {
                transform.position += new Vector3(-1f, 0, 0);
                pushFlg = true;
            }
            pushCnt++;
        }

        //----------------------------------------------
        //  水位の上下 (右スティック→LB, RBに変更)
        //----------------------------------------------
        // L Stick
        //float rsh = Input.GetAxis("R_Stick_H");
        float rsv = Input.GetAxis("R_Stick_V");
        float triggerLR = Input.GetAxis("L_R_Trigger");
        //if (rsv != 0)
        //{
        //    Debug.Log(rsv);
        //}
        bool pushFlg2 = false;

        // 正の値だと、
        if (Input.GetKey(KeyCode.UpArrow) || triggerLR > 0)
        {
            var ke = transform.position;
            ke.y += 1.5f * Time.deltaTime;
            transform.position = ke;

            pushFlg2 = true;
        }

        if (Input.GetKey(KeyCode.DownArrow) || triggerLR < 0)
        {
            var ke = transform.position;
            ke.y -= 1.5f * Time.deltaTime;
            transform.position = ke;

            pushFlg2 = true;
        }


        if (pushFlg2)
        {
            // SE再生
            if (!seFlg)
            {
                SoundManager.Instance.PlaySeEX("near_a_brook");
                seFlg = true;
            }
        }
        else
        {
            // SE停止
            if (seFlg)
            {
                Debug.Log("うまくいかんねｎ");
                //SoundManager.Instance.StopSe();
                SoundManager.Instance.StopSeEX("near_a_brook");
                seFlg = false;
            }
        }

        pushFlg2 = false;
    }
}
