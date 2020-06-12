using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;
using UnityEngine.SceneManagement;

public enum StageFlg {
    START,       // 開始時
    NOMAL,       // 通常営業 (ゲーム本編の操作可能)
    PAUSE_BEGIN, // ポーズ演出
    PAUSE_MENU,  // ポーズメニュー
    GAME_OVER,   // ゲームオーバー
    GAME_CLEAR,
    ENDSELECT,
    GAME_END,
    OTHER,
}

public class StageManager : SingletonMonoBehaviour<StageManager>, IUpdatable
{
    [SerializeField] private GameObject cameraObj;        //!< カメラ
    [SerializeField] private GameObject penguinObj;       //!< ペンギン野郎
    [SerializeField] private GameObject canvasData;       //!< 親Obj参照データ
    [SerializeField] private GameObject clearObj1;        //!< 「STAGE」
    [SerializeField] private GameObject clearObj2;        //!< 「CLEAR」
    [SerializeField] private GameObject stageEndButton;   //!< ステージエンドボタンUI
    private StageFlg stageFlg = StageFlg.START;           //!< ステージ状態flg
    private int stageNo = 1;                              //!< ステージ番号

    // Start is called before the first frame update
    void Start()
    {
        if (BaseSceneManager.Instance != null)
        {
            // シーンの初期化
            LoadingScene.Instance.InitScene();
            // シーンイン演出処理
            SceneChangeManager.Instance.SceneChangeIn(SceneChangeType.SLIDE_UP, 1.5f);

            // 現在どこのステージかの情報を取得
            stageNo = GameDataManager.Instance.GetNowStageNo();

            // BGMの再生
            switch (stageNo) {
                case 1:
                case 2:
                case 3:
                    //SoundManager.Instance.PlayBgm("Stage_Asa");
                    break;
                case 4:
                case 5:
                case 6:
                    //SoundManager.Instance.PlayBgm("Stage_Yugata");
                    break;
                case 7:
                case 8:
                case 9:
                    //SoundManager.Instance.PlayBgm("Stage_Yoru");
                    break;
                default:
                    SoundManager.Instance.PlayBgm("Stage_Asa");
                    break;
            }
        }
        else
        {
            // BGMの再生
            SoundManager.Instance.PlayBgm("Stage_Asa");
        }

        // START演出
        StartCoroutine(StartEvent());

        // デバッグ用
        //stgaeFlg = StageFlg.NOMAL;
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
        // 次のステージへ的な
        if (stageFlg == StageFlg.ENDSELECT)
        {
            // Aボタン
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown("joystick button 0"))
            {
                // セレクト画面へ
                SceneChangeManager.Instance.SceneChangeOut(SceneChangeType.FADE, 0.5f, "StageSelect");
                // SE
                SoundManager.Instance.PlaySe("凍る・コチーン");

                stageFlg = StageFlg.OTHER;
            }
        }
        //---------------------------------
        // GGGGGGGGG (※本番は必ず消すこと！！！！！！！！！！！！！！！！！！！！！！
        //---------------------------------
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    StartCoroutine(GoalEvent());
        //}
    }

    //====================================================================
    // スタート演出開始
    //====================================================================
    IEnumerator StartEvent()
    {
        // 時を止める
        Time.timeScale = 0.0f;

        yield return new WaitForSecondsRealtime(0.5f);

        // スタート演出用Objの生成
        GameObject obj = Instantiate(Resources.Load<GameObject>("StartEffect"), new Vector3(0,0,0), Quaternion.identity, canvasData.transform);
        StartEvent startEvent = obj.GetComponent<StartEvent>();

        // スタート演出開始
        yield return StartCoroutine(startEvent.StartEventing());

        // スタート演出用Objの破棄
        Destroy(obj);

        // ↓スタート演出の途中で動き出すように変更
        //// スタート演出終了後、flgをNOMALにしてゲームを開始する
        //stageFlg = StageFlg.NOMAL;
        //// そして時は動き出す
        //Time.timeScale = 1.0f;
    }

    //====================================================================
    // GAMEOVER開始
    //====================================================================

    //====================================================================
    // GOAL開始
    //====================================================================
    public IEnumerator GoalEvent()
    {
        // クリア演出へ
        stageFlg = StageFlg.GAME_CLEAR;

        // ステージセレクトカーソルを１つずらす
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.nextStageSelectPos();
        }

        // BGM・SE
        SoundManager.Instance.StopSe();
        SoundManager.Instance.StopSeEX("Step_EX");
        SoundManager.Instance.StopBgm();
        SoundManager.Instance.PlaySeEX("ファンファーレ2");

        // いらんUI削除
        canvasData.transform.Find("ALL_UI").gameObject.SetActive(false);
        GameObject item = GameObject.Find("ItemEX");
        if (item != null)
        {
            item.SetActive(false);
        }

        //!< 時間計測用
        float seconds = 1.0f;
        ProcessTimer processTimer = new ProcessTimer();

        //----------------------------------------------------
        // カメラ移動
        //----------------------------------------------------
        StartCoroutine(goal_camera());

        //----------------------------------------------------
        // 待機
        //----------------------------------------------------
        yield return new WaitForSecondsRealtime(0.5f);

        //----------------------------------------------------
        // カメラ移動終了
        // 「ステージクリア」の文字登場開始
        //----------------------------------------------------
        StartCoroutine(goal_clear1());

        yield return new WaitForSecondsRealtime(0.15f);

        GameObject obj2 = Instantiate(clearObj2, new Vector3( 470, 140, 0), Quaternion.identity);
        obj2.transform.SetParent(canvasData.transform, false);
        Vector3 pos2 = obj2.transform.localPosition;

        //!< ラジアンに変換 (20.0度→)
        float rad = 20.0f * Mathf.Deg2Rad;

        seconds = 1.0f;
        processTimer.Restart();

        while (seconds > processTimer.TotalSeconds)
        {
            float move = Easing.BackInOut(processTimer.TotalSeconds, seconds, 1800, 0.0f, 1);
            obj2.transform.localPosition = new Vector3(pos2.x - move * Mathf.Cos(rad), pos2.y + move * Mathf.Sin(rad), 0);

            // 継続
            yield return null;
        }

        // 調整用
        obj2.transform.localPosition = pos2;

        Debug.Log("StageClear!");

        //---------------------------------------
        //  ステージエンドUI
        //---------------------------------------
        GameObject obj3 = Instantiate(stageEndButton);
        obj3.transform.SetParent(canvasData.transform, false);

        // 「ステージクリア」文字登場終了
        // ボタンを押すとシーン遷移可能に
        stageFlg = StageFlg.ENDSELECT;
    }

    IEnumerator goal_camera()
    {
        //!< 時間計測用
        float seconds = 1.0f;
        ProcessTimer processTimer = new ProcessTimer();

        // 座標
        Vector3 penguinPos = penguinObj.transform.position;
        Vector3 cameraPos = cameraObj.transform.position;

        // ペンギン：わーいアニメーションへ

        // カメラ移動処理
        processTimer.Restart();
        seconds = 1.8f;
        while (seconds > processTimer.TotalSeconds)
        {
            Vector3 posEX;
            posEX.x = Easing.CubicOut(processTimer.TotalSeconds, seconds, cameraPos.x, penguinPos.x);
            posEX.y = Easing.CubicOut(processTimer.TotalSeconds, seconds, cameraPos.y, penguinPos.y + 0.60f);
            posEX.z = Easing.CubicOut(processTimer.TotalSeconds, seconds, cameraPos.z, penguinPos.z - 8.0f);

            cameraObj.transform.position = posEX;
            // 継続
            yield return null;
        }
        // 調整用
        cameraObj.transform.localScale = new Vector3(penguinPos.x, penguinPos.y + 0.60f, penguinPos.z - 8.0f);
    }

    IEnumerator goal_clear1()
    {
        GameObject obj1 = Instantiate(clearObj1, new Vector3(-470, 140, 0), Quaternion.identity);
        obj1.transform.SetParent(canvasData.transform, false);
        Vector3 pos1 = obj1.transform.localPosition;

        //!< ラジアンに変換 (25.0度→)
        float rad = 20.0f * Mathf.Deg2Rad;

        float seconds = 1.0f;
        ProcessTimer processTimer = new ProcessTimer();
        processTimer.Restart();

        while (seconds > processTimer.TotalSeconds)
        {
            float move = Easing.BackInOut(processTimer.TotalSeconds, seconds, 1800, 0.0f, 1);

            obj1.transform.localPosition = new Vector3(pos1.x + move * Mathf.Cos(rad), pos1.y + move * Mathf.Sin(rad), 0);

            // 継続
            yield return null;
        }

        // 調整用
        obj1.transform.localPosition = pos1;
    }

    //======================================
    // ステージflg関連
    //======================================
    public void SetFlg(StageFlg _flg)
    {
        stageFlg = _flg;
        if (stageFlg == StageFlg.NOMAL)
        {
            Time.timeScale = 1.0f;
        }
    }

    public StageFlg GetFlg()
    {
        return stageFlg;
    }

    //======================================
    // ステージ番号取得
    //======================================
    public int GetStageNo()
    {
        return stageNo;
    }
}
