﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GokUtil.UpdateManager;

public class PauseManager : SingletonMonoBehaviour<PauseManager>, IUpdatable
{
    [SerializeField] private GameObject canvasData;       //!< 親Obj参照データ
    [SerializeField] private GameObject textPrefab;       //!< 文字Objのプレハブ
    [SerializeField] private GameObject cursorPrefab;     //!< カーソル用Objのプレハブ

    /// <summary>
    /// ポーズ時生成Obj
    /// </summary>
    const int selectNum = 3;                              //!< 選択肢数

    GameObject back;                                      //!< 後ろの青背景Obj
    GameObject[] LeftEX = new GameObject[2];              //!< 左端Obj
    GameObject penguin;                                   //!< ペンギン絵Obj
    GameObject cursor;                                    //!< カーソル用Obj
    GameObject pauseText;                                 //!< PAUSE用テキストObj
    GameObject[] msg = new GameObject[selectNum];         //!< 選択肢用テキストobj

    bool isPause = false;                                 //!< ポーズflg
    int selectPos = 0;                                    //!< 選択肢位置 (0が一番上)
    string[] message = new string[]                       //!< 選択肢ワード
    {
        "つづける",
        "リトライ",
        "ステージセレクトへ",
    };

    // Start is called before the first frame update
    void Start()
    {
        //// 文字生成方法test
        //// １：Instantiateで生成するObject、座標を指定
        //obj[0] = Instantiate(textTest, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        //// ２：親をキャンバスに設定 (UIにするため)
        //obj[0].transform.SetParent(canvas.transform, false);
        //// ３：Textを取得して文字列の変更
        //Text t = obj[0].GetComponent<Text>();
        //t.text = "こんにちは";

        //obj= Instantiate(Panel, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
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
        // ポーズメニュー中の更新
        if (isPause)
        {
            // 関係ないときは動かせない
            if (StageManager.Instance.GetFlg() != StageFlg.PAUSE_MENU)
            {
                return;
            }

            //========================================
            // カーソル移動
            //========================================
            // カーソルを１つ上へ
            if (Input.GetKeyDown(KeyCode.W))
            {
                selectPos--;
                // 上端きたら下端へ
                if (selectPos < 0)
                {
                    selectPos = selectNum - 1;
                }
                // 座標を文字位置に合わせる
                Vector3 pos = new Vector3(msg[selectPos].transform.localPosition.x - 54, msg[selectPos].transform.localPosition.y - 3, 0);
                cursor.transform.localPosition = pos;
                // 色
                for(int i = 0; i < selectNum; i++)
                {
                    if (i == selectPos)
                    {
                        msg[i].GetComponent<Text>().color = Color.yellow;
                    }
                    else
                    {
                        msg[i].GetComponent<Text>().color = Color.white;
                    }
                }
            }
            // カーソルを１つ下へ
            if (Input.GetKeyDown(KeyCode.S))
            {
                selectPos++;
                // 下端きたら上端へ
                if (selectPos >= selectNum)
                {
                    selectPos = 0;
                }
                // 座標を文字位置に合わせる
                Vector3 pos = new Vector3(msg[selectPos].transform.localPosition.x - 54, msg[selectPos].transform.localPosition.y - 3, 0);
                cursor.transform.localPosition = pos;
                // 色
                for (int i = 0; i < selectNum; i++)
                {
                    if (i == selectPos)
                    {
                        msg[i].GetComponent<Text>().color = Color.yellow;
                    }
                    else
                    {
                        msg[i].GetComponent<Text>().color = Color.white;
                    }
                }
            }

            //========================================
            // 選択
            //========================================
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ClosePauseMenu();

                switch (selectPos)
                {
                    case 0:
                        // このまま閉じる
                        StageManager.Instance.SetFlg(StageFlg.NOMAL);
                        break;
                    case 1:
                        if (SceneChangeManager.Instance == null)
                        {
                            // このまま閉じる
                            StageManager.Instance.SetFlg(StageFlg.NOMAL);
                            break;
                        }
                        // リスタート (同じシーンを読み込み直す)
                        SceneChangeManager.Instance.SceneChangeOut(SceneChangeType.FADE, 0.2f, LoadingScene.Instance.GetNowScene());
                        break;
                    case 2:
                        if (SceneChangeManager.Instance == null)
                        {
                            // このまま閉じる
                            StageManager.Instance.SetFlg(StageFlg.NOMAL);
                            break;
                        }
                        // セレクト画面へ
                        SceneChangeManager.Instance.SceneChangeOut(SceneChangeType.FADE, 0.2f, "StageSelect");
                        break;
                    default:
                        // ありえへん
                        Debug.Log("Select_ERROR");
                        break;
                }
            }
            // Escapeでとりあえず閉じる
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClosePauseMenu();
                // このまま閉じる
                StageManager.Instance.SetFlg(StageFlg.NOMAL);
            }

            if (Input.GetKeyDown(KeyCode.K))
            {

            }
        }
        // まだポーズメニュー開いてないよ
        else
        {
            // 関係ないときは動かせない
            if (StageManager.Instance.GetFlg() != StageFlg.NOMAL)
            {
                return;
            }

            // 開くよ
            //========================================
            // 選択
            //========================================
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // ポーズ中に切り替え
                isPause = true;
                StageManager.Instance.SetFlg(StageFlg.PAUSE_MENU);

                // ザ・ワールド
                Time.timeScale = 0f;

                // もしポーズするたびに選択カーソル位置を一番上にリセットするならコレ
                selectPos = 0;

                // Obj生成、アニメーション開始
                CreatePauseObj();
            }
        }
    }

    //========================================
    // ポーズ用Objの生成・アニメの開始
    //========================================
    void CreatePauseObj()
    {
        //=================================
        // 薄い青背景Obj生成
        //=================================
        back = new GameObject("BACK_BLUE");
        back.transform.SetParent(canvasData.transform, false);
        //back.transform.localScale = new Vector3(1.0f, 1.0f, 1);
        // 色合い
        Image image = back.AddComponent<Image>();
        image.color = new Color(0.0f, 137.0f / 255.0f, 255.0f / 255.0f, 50.0f / 255.0f);
        // 画面全体の大きさに合わせる
        var rect = back.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.0f, 0.0f);
        rect.anchorMax = new Vector2(1.0f, 1.0f);
        rect.offsetMin = new Vector2(0.0f, 0.0f);  // left, bottom
        rect.offsetMax = new Vector2(0.0f, 0.0f);  // right, up

        //=================================
        // ペンギンObj生成
        //=================================
        penguin = new GameObject("BigPenguin");
        penguin.transform.SetParent(canvasData.transform, false);
        penguin.transform.localPosition = new Vector3(442.0f, -150.0f, 0.0f);    // 510,-28
        penguin.transform.localScale = new Vector3(14.0f, 14.0f, 1);    // 18,18
        penguin.transform.Rotate(0.0f, 0.0f, 3.0f, Space.World);
        // 画像
        Sprite afterPic = Resources.Load<Sprite>("Texture/LOADING/roading_01");
        image = penguin.AddComponent<Image>();
        image.sprite = afterPic;
        // 色合い
        image.color = new Color(0.0f, 138.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);
        // 画面全体の大きさに合わせる
        var rect2 = penguin.GetComponent<RectTransform>();
        rect2.anchorMin = new Vector2(0.0f, 0.5f);
        rect2.anchorMax = new Vector2(0.0f, 0.5f);
        //rect2.offsetMin = new Vector2(100.0f, 100.0f);  // left, bottom 311,100
        //rect2.offsetMax = new Vector2(100.0f, 100.0f);  // right, top 100,-29

        //=================================
        // 左端Obj (白) 生成
        //=================================
        LeftEX[1] = new GameObject("LeftWall_White");
        LeftEX[1].transform.SetParent(canvasData.transform, false);
        //LeftEX[1].transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        LeftEX[1].transform.localScale = new Vector3(1.0f, 1.0f, 1);
        LeftEX[1].transform.Rotate(0.0f, 0.0f, 3.0f, Space.World);
        // 色合い
        image = LeftEX[1].AddComponent<Image>();
        image.color = Color.white;
        // 画面全体の大きさに合わせる
        var rect3 = LeftEX[1].GetComponent<RectTransform>();
        rect3.anchorMin = new Vector2(0.0f, 0.0f);
        rect3.anchorMax = new Vector2(0.0f, 1.0f);
        rect3.offsetMin = new Vector2(130.0f, 0.0f);  // left, bottom 87
        rect3.offsetMax = new Vector2(150.0f, 0.0f);  // right, top 200

        //=================================
        // カーソル生成
        //=================================
        cursor = new GameObject("Cursor");
        cursor.transform.SetParent(canvasData.transform, false);
        cursor.transform.localPosition = new Vector3(106.0f, 180.0f, 0.0f);
        cursor.transform.localScale = new Vector3(6.0f, 0.45f, 1);
        cursor.transform.Rotate(0.0f, 0.0f, 3.0f, Space.World);
        // 色合い
        image = cursor.AddComponent<Image>();
        image.color = new Color(255.0f / 255.0f, 76.0f / 255.0f, 76.0f / 255.0f, 230.0f / 255.0f);
        // 画面全体の大きさに合わせる
        var rect4 = cursor.GetComponent<RectTransform>();
        rect4.anchorMin = new Vector2(0.0f, 0.0f);
        rect4.anchorMax = new Vector2(0.0f, 0.0f);

        //=================================
        // PAUSE生成
        //=================================
        pauseText=Instantiate(textPrefab, new Vector3(435.0f, 355.0f, 0.0f), Quaternion.identity);
        pauseText.name = "Text_PAUSE";
        //pauseText = new GameObject("Text_PAUSE");
        pauseText.transform.SetParent(canvasData.transform, false);
        //pauseText.transform.localPosition = new Vector3(207.0f, 265.0f, 0.0f);
        pauseText.transform.localScale = new Vector3(0.80f, 0.80f, 1);
        pauseText.transform.Rotate(0.0f, 0.0f, 3.0f, Space.World);
        // 画面全体の大きさに合わせる
        var rect5 = pauseText.GetComponent<RectTransform>();
        rect5.anchorMin = new Vector2(0.0f, 0.0f);
        rect5.anchorMax = new Vector2(0.0f, 0.0f);
        // 色合い
        Text t = pauseText.GetComponent<Text>();
        t.color = Color.white;
        t.text = "PAUSE";

        //=================================
        // 選択肢生成
        //=================================
        for(int i = 0; i < selectNum; i++)
        {
            msg[i] = Instantiate(textPrefab, new Vector3(250.0f + 20 * i, 230.0f - 60 * i, 0.0f), Quaternion.identity);
            msg[i].name = "Text_Select" + i;
            //pauseText = new GameObject("Text_PAUSE");
            msg[i].transform.SetParent(canvasData.transform, false);
            //pauseText.transform.localPosition = new Vector3(207.0f, 265.0f, 0.0f);
            msg[i].transform.localScale = new Vector3(0.2f, 0.2f, 1);
            msg[i].transform.Rotate(0.0f, 0.0f, 3.0f, Space.World);
            // 画面全体の大きさに合わせる
            var rect6 = msg[i].GetComponent<RectTransform>();
            rect6.anchorMin = new Vector2(0.0f, 0.0f);
            rect6.anchorMax = new Vector2(0.0f, 0.0f);
            // 色合い
            Text t2 = msg[i].GetComponent<Text>();
            t2.alignment = TextAnchor.MiddleLeft;
            t2.text = message[i];
            // 最初の選択肢
            if (i == selectPos)
            {
                t2.color = Color.yellow;
            }
        }

        //=================================
        // 左端Obj (青) 生成
        //=================================
        LeftEX[0] = new GameObject("LeftWall_Blue");
        LeftEX[0].transform.SetParent(canvasData.transform, false);
        LeftEX[0].transform.localScale = new Vector3(2.6f, 1.05f, 1);
        LeftEX[0].transform.Rotate(0.0f, 0.0f, 3.0f, Space.World);
        // 色合い
        image = LeftEX[0].AddComponent<Image>();
        image.color = new Color(0.0f, 138.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);
        // 画面全体の大きさに合わせる
        var rect7 = LeftEX[0].GetComponent<RectTransform>();
        rect7.anchorMin = new Vector2(0.0f, 0.0f);
        rect7.anchorMax = new Vector2(0.0f, 1.0f);
        rect7.offsetMin = new Vector2(-50.0f, 0); // left, bottom 31
        rect7.offsetMax = new Vector2(50.0f, 0);  // right, top 100

        //LeftEX[0].transform.localPosition = new Vector3(0, LeftEX[0].transform.localPosition.y - Screen.height, 0);
        //Debug.Log(Screen.width / 2);

        // カーソル合わせ
        Vector3 pos = new Vector3(msg[selectPos].transform.localPosition.x - 54, msg[selectPos].transform.localPosition.y - 3, 0);
        cursor.transform.localPosition = pos;
    }

    //========================================
    // ポーズメニューを閉じる
    //========================================
    void ClosePauseMenu()
    {
        // ポーズ解除
        isPause = false;
        // 通常営業
        Time.timeScale = 1f;

        //--------------------------------------
        //  ポーズ用のObj削除
        //--------------------------------------
        Destroy(pauseText);

        // メッセージObj削除
        for (int i = 0; i < selectNum; i++)
        {
            Destroy(msg[i]);
        }
        Destroy(pauseText);
        Destroy(cursor);
        Destroy(penguin);
        for(int i = 0; i < 2; i++)
        {
            Destroy(LeftEX[i]);
        }
        Destroy(back);
    }

    //========================================
    // ポーズ状態を取得...
    //========================================
    public bool GetisPause()
    {
        return isPause;
    }
}
