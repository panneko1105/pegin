using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @date 2020/05/01 [今後修正予定]
//
// GAMEOVER画面への遷移
//

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    public GameObject fadePanel;
    Fade fadeSC;
    bool flg = true;

    // Start is called before the first frame update
    void Start()
    {
        // BGMの再生
        SoundManager.Instance.PlayBgm("BGM_Test01");

        // BaseSceneのいらないものを消す
        BaseSceneManager.Instance.SetObject(false);
        fadeSC = fadePanel.GetComponent<Fade>();
        // フェードイン処理
        fadeSC.StartFadeIn(0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (flg)
        {

        }
    }
}
