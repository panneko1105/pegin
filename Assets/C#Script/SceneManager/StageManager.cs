using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;
using UnityEngine.SceneManagement;

// @date 2020/05/29 [今後修正予定]
//
// GOALへの遷移
//

public enum StageFlg {
    START,       // 開始時
    NOMAL,       // 通常営業 (ゲーム本編の操作可能)
    PAUSE_BEGIN, // ポーズ演出
    PAUSE_MENU,  // ポーズメニュー
    GAME_OVER,   // ゲームオーバー
    GAME_CLEAR,
}

public class StageManager : SingletonMonoBehaviour<StageManager>, IUpdatable
{
    [SerializeField] private GameObject canvasData;       //!< 親Obj参照データ
    private StageFlg stageFlg = StageFlg.START;           //!< ステージ状態flg
    private int stageNo = 1;                              //!< ステージ番号

    // Start is called before the first frame update
    void Start()
    {
        // BGMの再生
        SoundManager.Instance.PlayBgm("BGM_Test01");

        if (BaseSceneManager.Instance != null)
        {
            // シーンの初期化
            LoadingScene.Instance.InitScene();
            // シーンイン演出処理
            SceneChangeManager.Instance.SceneChangeIn(SceneChangeType.SLIDE_UP, 1.5f);

            // 現在どこのステージかの情報を取得
            stageNo = GameDataManager.Instance.GetNowStageNo();
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
        // 
        if (stageFlg == StageFlg.NOMAL)
        {

        }
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

        // ↓スタート演出の途中で動き出す
        //// スタート演出終了後、flgをNOMALにしてゲームを開始する
        //stageFlg = StageFlg.NOMAL;
        //// そして時は動き出す
        //Time.timeScale = 1.0f;
    }

    //====================================================================
    // GAMEOVER開始
    //====================================================================
    public void StartGameOver()
    {
        // ゲームオーバーへ
    }

    //====================================================================
    // GOAL開始
    //====================================================================
    public void StartGoal()
    {
        // ゲームオーバーへ
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
