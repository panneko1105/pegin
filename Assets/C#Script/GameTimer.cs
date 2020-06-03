using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GokUtil.UpdateManager;

//
// ゲーム用タイマー表示
// 現在2桁までのみ対応
//
    
public class GameTimer : SingletonMonoBehaviour<GameTimer>, IUpdatable
{
    [SerializeField] private GameObject[] timerObj = new GameObject[2];   //!< [0]が1桁目、[1]が2桁目

    ProcessTimer processTimer;          //!< 時間計測用
    int timer;                          //!< 経過時間格納
    const int maxCnt = 30;              //!< 最大カウント数
    bool isCntDownFlg = false;          //!< カウントダウン処理を行うかのflg
    bool soloFlg = false;               //!< 1桁のみか否かを判定flg
    public bool endFlg                  //!< カウント終了flg
    {
        get
        { return endFlg; }
        set
        { endFlg = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartTimer();
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
        // カウントダウン処理
        if (isCntDownFlg)
        {
            //-------------------
            // 時間計測
            //-------------------
            int tmpCnt = processTimer.Seconds;

            // もしカウント数値が異なるなら表示更新
            if (timer != tmpCnt)
            {
                // 1桁突入
                if (!soloFlg && tmpCnt < 10)
                {
                    // 座標を中央に変更
                    timerObj[0].transform.position = new Vector3(0, timerObj[0].transform.position.y, 0);
                    // 2桁目を処刑
                    timerObj[1].SetActive(false);

                    // 切替
                    soloFlg = true;
                }

                // 1桁目の更新
                if((timer % 10) != (tmpCnt % 10))
                {
                    // 画像更新
                    ChangeImage(1, tmpCnt % 10);
                }

                // 2桁目の更新
                if (tmpCnt >= 10 && (timer / 10) != (tmpCnt / 10))
                {
                    // 画像更新
                    ChangeImage(2, tmpCnt / 10);
                }

                // 数値の更新
                timer = tmpCnt;

                //------------------------
                //  カウント終了
                //------------------------
                if (timer <= 0)
                {
                    //timer = 0;
                    isCntDownFlg = false;
                    endFlg = true;
                }
            }
        }
    }

    //======================================
    //  画像更新 (i：「1」桁目～指定)
    //======================================
    void ChangeImage(int i, int num)
    {
        if(i < 1)
        {
            i = 1;
        }
        else if (i > 2)
        {
            i = 2;
        }

        if(num < 0)
        {
            num = 0;
        }
        else if(num > 9)
        {
            num = 9;
        }

        //画像切替
        Sprite afterPic = Resources.Load<Sprite>("Texture/Timer/" +  num);
        Image image = timerObj[i - 1].GetComponent<Image>();
        image.sprite = afterPic;
    }

    //======================================
    //  カウントダウンスターティン！
    //======================================
    public void StartTimer()
    {
        // 計測開始
        processTimer.Restart();
        timer = processTimer.Seconds;

        // flg設定
        isCntDownFlg = true;
        soloFlg = false;
        endFlg = false;

        // ObjのOn
        //timerObj[0].SetActive(true);
        // Objの座標設定
    }
}
