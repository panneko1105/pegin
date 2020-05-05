using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GokUtil.UpdateManager;


public class TitleManager : SingletonMonoBehaviour<TitleManager>, IUpdatable
{
    public GameObject fadePanel;
    Fade fadeSC;
    bool flg = true;
    [SerializeField] SceneObject m_nextScene;       //!< 次のシーン先をInspector上で指定できるよ

    // Use this for initialization
    void Start()
    {
        // BaseSceneのいらないものを消す
        BaseSceneManager.Instance.SetObject(false);
        // アクティブシーンを切り替え
        Scene scene = SceneManager.GetSceneByName(LoadingScene.Instance.GetNowScene());
        SceneManager.SetActiveScene(scene);

        // フェード取得
        fadeSC = fadePanel.GetComponent<Fade>();
        // フェードイン処理
        fadeSC.StartFadeIn(1.0f);
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
        if (flg)
        {
            if (Input.GetMouseButton(0))
            {
                flg = false;
                // シーン遷移
                //LoadingScene.Instance.LoadScene(m_nextScene);
                fadeSC.StartFadeOut("Stage1", 0.5f);
            }
        }   
    }
}
