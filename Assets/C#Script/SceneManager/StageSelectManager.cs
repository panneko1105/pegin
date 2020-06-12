﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public enum StageSelectFlg
{
    START,          // 0:開始時
    START_STAR,     // ステージ後、☆取得処理 (更新してない場合はSTARTから開始)
    NOMAL,          // 通常営業 (ゲーム本編の操作可能)
    MOVE,           // 移動時故に他の操作不可
    LOCK_LIFT,      // 鍵開け

    SCENE_CHANGE,   // シーン遷移故に操作不可
}

public class StageSelectManager : SingletonMonoBehaviour<StageSelectManager>, IUpdatable
{
    [SerializeField] float sceneInSpeed = 1.2f;   //!< イン演出のspeed (参考：1.2f)
    [SerializeField] int firstSelect = 1;         //!< ステージ選択カーソル初期位置 (基本は１じゃね)
    [SerializeField] float cursorSpeed = 0.16f;   //!< ステージ選択カーソル移動速度
    const float panelSpeed = 0.15f;               //!< パネル生成アニメーションの速度
    int selectCursortpos = 1;                      //!< ステージ選択カーソル箇所 (1～)
    const int stageMax = 8;                       //!< ステージ最大数 (※)
    StageSelectFlg selectFlg = StageSelectFlg.START;

    /* ステージセレクト位置設定関連 */
    [SerializeField] private GameObject cursorObj;        //!< カーソル用Obj
    [SerializeField] GameObject[] mapPlane = new GameObject[stageMax];   //!< 各ステージの足場のObj
    GameObject selectPanelObj;                    //!< セレクトパネル用Obj
    IEnumerator panelAnimEvent;                   //!< パネルアニメーションイベント用

    // Start is called before the first frame update
    void Start()
    {
        // BGMの再生
        SoundManager.Instance.PlayBgm("StageSelect");
        selectCursortpos = firstSelect;

        if (BaseSceneManager.Instance != null)
        {
            // シーンの初期化
            LoadingScene.Instance.InitScene();
            // シーンイン演出処理
            SceneChangeManager.Instance.SceneChangeIn(SceneChangeType.SLIDE_RIGHT, sceneInSpeed);
            Debug.Log(GameDataManager.Instance.StageSelectPos);
            // カーソル位置の読み込み
            selectCursortpos = GameDataManager.Instance.StageSelectPos;
        }

        // カーソル位置を合わせる
        cursorObj.transform.position = mapPlane[selectCursortpos - 1].transform.position;

        // セレクトパネルを更新
        SetFirstPanel();
        //Invoke("SetFirstPanel", 0.1f);
    }

    void SetFirstPanel()
    {
        ChangeSelectPanel(selectCursortpos);
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
        bool isChageCursor = false;

        // L Stick
        float lsh = Input.GetAxis("L_Stick_H");
        float lsv = Input.GetAxis("L_Stick_V");
        //if (lsh != 0)
        //{
        //    Debug.Log(lsh);
        //}
        // 十字キー
        float dph = Input.GetAxis("D_Pad_H");
        float dpv = Input.GetAxis("D_Pad_V");

        // 通常営業時のみ操作狩野英孝
        if (selectFlg == StageSelectFlg.NOMAL)
        {
            //========================================
            // カーソル移動
            //========================================
            // ←
            if (Input.GetKeyDown(KeyCode.A) || lsh < -0.1f || dph < -0.0f)
            {
                selectCursortpos--;
                // 左端
                if (selectCursortpos < 1)
                {
                    selectCursortpos = 1;
                }
                else
                {
                    if (!isChageCursor)
                    {
                        // カーソル移動開始
                        // 左と右のダブりは禁物でっせ
                        isChageCursor = true;
                        selectFlg = StageSelectFlg.MOVE;
                        StartCoroutine(StartCursorMove(selectCursortpos + 1, selectCursortpos));
                    }
                }
            }
            // →
            if (Input.GetKeyDown(KeyCode.D) || lsh > 0.1f || dph > 0.0f)
            {
                selectCursortpos++;
                // 左端
                if (selectCursortpos > stageMax)
                {
                    selectCursortpos = stageMax;
                }
                else
                {
                    // カーソル移動開始
                    // 左と右のダブりは禁物でっせ
                    if (!isChageCursor)
                    {
                        StartCoroutine(StartCursorMove(selectCursortpos - 1, selectCursortpos));
                        selectFlg = StageSelectFlg.MOVE;
                        isChageCursor = true;
                    }
                }
            }

            //========================================
            // 決定 (Aボタン)
            //========================================
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
            {
                selectFlg = StageSelectFlg.SCENE_CHANGE;
                // SE
                SoundManager.Instance.PlaySe("small_ice1");
                // カーソル位置の保存
                GameDataManager.Instance.StageSelectPos = selectCursortpos;
                // シーン遷移
                //
                // デバッグ用！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
                //
                //if(selectCursortpos > 7)
                //{
                //    selectCursortpos = 7;
                //}
                SceneChangeManager.Instance.SceneChangeOut(SceneChangeType.FADE, 0.5f, "Stage" + selectCursortpos);
            }

            //========================================
            // タイトルに戻る (Bボタン)
            //========================================
            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown("joystick button 1"))
            {
                selectFlg = StageSelectFlg.SCENE_CHANGE;
                // SE
                SoundManager.Instance.PlaySe("pa1");
                // シーン遷移
                SceneChangeManager.Instance.SceneChangeOut(SceneChangeType.FADE, 0.4f, "Title");
            }

            isChageCursor = false;
        }
    }

    //========================================
    // 1:カーソル移動
    //========================================
    IEnumerator StartCursorMove(int _nowCursorPos, int _nextCursorPos)
    {
        selectFlg = StageSelectFlg.MOVE;

        // 既存のパネルを破棄
        StopCoroutine(panelAnimEvent);
        Destroy(selectPanelObj);

        //!< スクリプトを取得
        StageSelectCursor stageSelectCursorCS = cursorObj.GetComponent<StageSelectCursor>();

        //!< カーソル移動呼び出し
        Vector3 firstPos = mapPlane[_nowCursorPos - 1].transform.position;
        Vector3 lastPos = mapPlane[_nextCursorPos - 1].transform.position;
        yield return StartCoroutine(stageSelectCursorCS.MoveAnimation(cursorSpeed, firstPos, lastPos));

        // SE
        SoundManager.Instance.PlaySeEX("pa1");

        // flg切り替え (操作可能に)
        selectFlg = StageSelectFlg.NOMAL;

        // カーソル移動が終了次第、新規セレクトパネル生成
        ChangeSelectPanel(selectCursortpos);
    }

    //========================================
    // 2:セレクトパネル情報更新
    //========================================
    void ChangeSelectPanel(int _stageNo)
    {
        //----------------------------------
        //!< パネルを生成
        //----------------------------------
        GameObject prefab = (GameObject)Resources.Load("SELECT_PANEL_SET");
        selectPanelObj = Instantiate(prefab, cursorObj.transform.position, Quaternion.identity);
        // キャンバスに配置
        //selectPanelObj.transform.SetParent(GameObject.Find("Canvas").transform);
        selectPanelObj.transform.SetParent(cursorObj.transform);
        // 角度傾き
        //selectPanelObj.transform.Rotate(0.0f, 0.0f, 4.0f, Space.World);
        //!< パネルにステージ情報をセット
        SelectPanelManager selectPanelManager = selectPanelObj.GetComponent<SelectPanelManager>();
        selectPanelManager.SetInfo(selectCursortpos);

        // アニメーション開始
        //yield return StartCoroutine(selectPanelManager.MoveAnimation(0.1f));
        panelAnimEvent = selectPanelManager.MoveAnimation(panelSpeed);
        StartCoroutine(panelAnimEvent);

        //  (操作可能に)
        selectFlg = StageSelectFlg.NOMAL;
    }
}
