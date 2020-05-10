using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;
using UnityEngine.SceneManagement;

// @date 2020/05/06 [今後修正予定]
//
// １つ前のシーンに戻る
//

public class GameOverManager : SingletonMonoBehaviour<GameOverManager>, IUpdatable
{
    // Start is called before the first frame update
    void Start()
    {
        // BaseSceneのいらないものを消す
        BaseSceneManager.Instance.SetObject(false);
        // アクティブシーンを切り替え
        Scene scene = SceneManager.GetSceneByName(LoadingScene.Instance.GetNowScene());
        SceneManager.SetActiveScene(scene);
        // フェード取得
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
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadingScene.Instance.LoadScene(LoadingScene.Instance.GetNowScene());
        }
    }
}
