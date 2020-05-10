using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;
using UnityEngine.SceneManagement;

// @date 2020/05/06 [今後修正予定]
//
// GAMEOVER画面への遷移
//

public class StageManager : SingletonMonoBehaviour<StageManager>, IUpdatable
{
    [SerializeField] private GameObject fadePanel;
    Fade fadeSC;
    bool flg = true;

    // Start is called before the first frame update
    void Start()
    {
        // BGMの再生
        SoundManager.Instance.PlayBgm("BGM_Test01");

        // BaseSceneのいらないものを消す
        BaseSceneManager.Instance.SetObject(false);
        // アクティブシーンを切り替え
        Scene scene = SceneManager.GetSceneByName(LoadingScene.Instance.GetNowScene());
        SceneManager.SetActiveScene(scene);
        // フェード取得
        fadeSC = fadePanel.GetComponent<Fade>();
        // フェードイン処理
        fadeSC.StartFadeIn(0.5f);
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
        if (flg)
        {

        }
    }
}
