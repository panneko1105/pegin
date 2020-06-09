using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadingScene : SingletonMonoBehaviour<LoadingScene>
{
    private static string preScene = "Title";                   //!< １つ前のシーン先を保存 (前のシーンに戻りたいときに使用)
    private static string nowScene = "Title";                   //!< 現在のシーン先を保存
    //bool isLoad = false;
    const float waitSeconds = 0.5f;                             //!< 最低限待たせる時間[s]

    // Start is called before the first frame update
    void Start()
    {
        // 
        Debug.Log("GAME_START");
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    //========================================
    // シーンの読込 (初回のみ)
    //========================================
    public void FirstLoadScene(string sceneName)
    {
        // 遷移前にシーン名保存
        Debug.Log(nowScene);
        preScene = nowScene;
        nowScene = sceneName;

        // 現在のシーンを破棄 (初回なのでいらない)
        // 関数別に作らずif()文分岐でええかもね→分かりにくいめんどいまぎらわしい現状維持で
        //SceneManager.UnloadSceneAsync(preScene);

        // 読み込み処理開始 (非同期)
        StartCoroutine(LoadNextScene(sceneName));
    }

    //========================================
    // シーンの読込
    //========================================
    public void LoadScene(string sceneName)
    {
        // 遷移前にシーン名保存
        Debug.Log(nowScene);
        preScene = nowScene;
        nowScene = sceneName;

        // 現在のシーンを破棄 (BaseSceneだけ残る)
        SceneManager.UnloadSceneAsync(preScene);

        // 読み込み処理開始 (非同期)
        StartCoroutine(LoadNextScene(sceneName));
    }

    IEnumerator LoadNextScene(string sceneName)
    {
        //==========================================================
        // 遷移前の演出挟むならココ！
        //==========================================================
        BaseSceneManager.Instance.SetObject(true);

        // 計測開始
        ProcessTimer processTimer = new ProcessTimer();
        processTimer.Restart();

        Debug.Log("ロード先 : " + sceneName);
        //Debug.Log("現在読み込まれてるシーンの数 : " + SceneManager.sceneCount + " 個");

        //foreach (Scene s in SceneManager.GetAllScenes()) { Debug.Log(s.name); }

        // 非同期でロード開始させる (追加)
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // ロード完了時に自動的に遷移させないようにする
        async.allowSceneActivation = false;

        // 確認用
        //Debug.Log("allowSceneActivation : " + async.allowSceneActivation);
        //Debug.Log("start while : " + async.progress);

        // ロードが完了していない間ループする
        // ※allowSceneActivationがfalseの場合、progressは「0.9f」、isDoneは「false」で終わるので注意！
        // ※演出などで確実に待ちを入れたい場合は ( async.progress < 0.9f || 読み込み時間 < 確実に待たせたい時間[] )
        while (async.progress < 0.9f || processTimer.TotalSeconds < waitSeconds)
        {
            // ローディングの進捗状況
            //Debug.Log("while : " + async.progress);
            // シーンの読み込みが終わったらtrueになるよ...と思っていた時期が私にもありました。
            //Debug.Log("isDone : " + async.isDone);

            yield return null;
        }

        // ロード完了！
        Debug.Log("Loading Completed!");

        //==========================================================
        // 遷移直前の演出挟むならココ！ (BaseSceneでのFadeOutなど)
        //==========================================================
        //
        //
        //
        // 遷移許可
        async.allowSceneActivation = true;

        // 現在のステージNo.を更新
        switch (sceneName) {
            case "Stage1":
                GameDataManager.Instance.SetNowStageNo(1);
                break;
            case "Stage2":
                GameDataManager.Instance.SetNowStageNo(2);
                break;
            case "Stage3":
                GameDataManager.Instance.SetNowStageNo(3);
                break;
            case "Stage4":
                GameDataManager.Instance.SetNowStageNo(4);
                break;
            case "Stage5":
                GameDataManager.Instance.SetNowStageNo(5);
                break;
            case "Stage6":
                GameDataManager.Instance.SetNowStageNo(6);
                break;
            case "Stage7":
                GameDataManager.Instance.SetNowStageNo(7);
                break;
            case "Stage8":
                GameDataManager.Instance.SetNowStageNo(8);
                break;
            case "Stage9":
                GameDataManager.Instance.SetNowStageNo(9);
                break;
        }


        // ※ここでBaseSceneのObjを消した時にはまだシーン遷移されず、一瞬虚無フィールドが映ってしまう。
        // 　フェードアウトすればたぶん問題ないが、できれば修正したい。→各シーンのStart()でfalse処理を行うと綺麗にいった...それでいいのか
        //BaseSceneManager.Instance.ObjectSet(false);
    }

    //========================================
    // 指定のシーンが既にあるかどうかの確認
    //========================================
    bool ContainsScene(string sceneName)
    {
        // 現在読み込まれているシーン数だけループ
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            // 読み込まれているシーン名を取得
            if (SceneManager.GetSceneAt(i).name == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    //========================================
    // 現在シーンの確認
    //========================================
    public string GetNowScene()
    {
        return nowScene;
    }

    //========================================
    // １つ前のシーンの確認
    //========================================
    public string GetPreScene()
    {
        return preScene;
    }

    //========================================
    // シーン初期化まとめ
    //========================================
    public void InitScene()
    {
        // BaseSceneのいらないものを消す
        BaseSceneManager.Instance.SetObject(false);
        // アクティブシーンを切り替え
        Scene scene = SceneManager.GetSceneByName(GetNowScene());
        SceneManager.SetActiveScene(scene);
    }
}
