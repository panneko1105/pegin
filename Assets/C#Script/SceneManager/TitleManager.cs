using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @date 2020/05/01 [今後修正予定]
//
// 演出しながらシーン遷移。
//

public class TitleManager : SingletonMonoBehaviour<TitleManager>
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
        fadeSC = fadePanel.GetComponent<Fade>();
        // フェードイン処理
        fadeSC.StartFadeIn(1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (flg)
        {
            if (Input.GetMouseButton(0))
            {
                flg = false;
                // シーン遷移
                LoadingScene.Instance.LoadScene(m_nextScene);
                //fadeSC.StartFadeOut("Stage1", 1.0f);
            }
        }   
    }
}
