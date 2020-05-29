using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;
using UnityEngine.SceneManagement;

public class GameOverManager : SingletonMonoBehaviour<GameOverManager>, IUpdatable
{
    // Start is called before the first frame update
    void Start()
    {
        // BGMの再生
        //SoundManager.Instance.PlayBgm("BGM_Test01");
        // シーンの初期化
        LoadingScene.Instance.InitScene();
        // シーンイン演出処理
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
            // １つ前のシーンに戻る
            LoadingScene.Instance.LoadScene(LoadingScene.Instance.GetPreScene());
        }
    }
}
