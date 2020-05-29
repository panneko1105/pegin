﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;


public class TitleManager : SingletonMonoBehaviour<TitleManager>, IUpdatable
{
    [SerializeField] SceneObject m_nextScene;       //!< 次のシーン先をInspector上で指定できるよ
    bool flg = true;

    // Use this for initialization
    void Start()
    {
        // BGMの再生
        //SoundManager.Instance.PlayBgm("BGM_Test01");
        // シーンの初期化
        LoadingScene.Instance.InitScene();
        // シーンイン演出処理
        SceneChangeManager.Instance.SceneChangeIn(SceneChangeType.FADE, 0.5f);

        // 仮取得処理
        //GameDataManager.Instance.SaveItemFlg(1, 2);
        //GameDataManager.Instance.SaveItemFlg(1, 3);
        //GameDataManager.Instance.SaveItemFlg(3, 1);
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
            if (Input.GetKeyDown(KeyCode.Return))
            {
                flg = false;
                // シーン遷移
                SceneChangeManager.Instance.SceneChangeOut(SceneChangeType.FADE, 0.5f, m_nextScene);
            }
        }
    }
}