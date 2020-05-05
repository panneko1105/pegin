using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GokUtil.UpdateManager;

// @date 2020/05/06
//
// □１つ前のシーンに戻る機能の実装。
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
    }

    void OnEnable()
    {
        UpdateManager.AddUpdatable(this);
    }

    void OnDisable()
    {
        UpdateManager.RemoveUpdatable(this);
    }

    // Use this for initialization
    public void UpdateMe()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            //SceneManager.LoadScene(TransitionManager.previous);
            // １つ前のシーンに戻る
            LoadingScene.Instance.LoadScene(LoadingScene.Instance.GetPreScene());
        }
    }
}
