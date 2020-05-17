using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public class StageSelectManager : SingletonMonoBehaviour<StageSelectManager>, IUpdatable
{
    [SerializeField] float sceneInSpeed = 1.2f;   //!< イン演出のspeed (参考：1.2f)
    [SerializeField] int firstSelect = 1;    //!< ステージ選択カーソル初期位置 (基本は１じゃね)
    int selecCursortpos = 1;                 //!< ステージ選択カーソル箇所

    // Start is called before the first frame update
    void Start()
    {
        // BGMの再生
        //SoundManager.Instance.PlayBgm("BGM_Test01");
        // シーンの初期化
        LoadingScene.Instance.InitScene();
        // シーンイン演出処理
        SceneChangeManager.Instance.SceneChangeIn(SceneChangeType.SLIDE_RIGHT, sceneInSpeed);
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

    }
}
