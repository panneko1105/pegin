﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

// @date 2020/05/06 [今後修正予定]
//
// NowLoadingの動きとか
//
// Item情報保存
//

public class BaseSceneManager : SingletonMonoBehaviour<BaseSceneManager>, IUpdatable
{
    private bool[] isItemGetFlg = new bool[3];           //!< 取得の有無 (今後ステージ分これ用意したい)
    [SerializeField] GameObject canvas;                  //!< キャンバス情報
    [SerializeField] SceneObject firstScene;             //!< 開始するシーンを指定 (まぁ普通はTitleからだよね)

    // Start is called before the first frame update
    void Start()
    {
        // 開始
        LoadingScene.Instance.FirstLoadScene(firstScene);
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

    //========================================
    // アイテム取得処理 ※1～の指定
    //========================================
    public void SetItemGetFlg(int a)
    {
        // 例外防止
        if (a < 1)
        {
            a = 1;
        }
        else if (a > 3)
        {
            a = 3;
        }
        // 取得扱いに
        isItemGetFlg[a - 1] = true;
    }

    //========================================
    // NowLoading画面のObjの有無
    //========================================
    public void SetObject(bool isUse)
    {
        // falseになると存在を確認できないらしく、直接trueにできなかった...。
        // 仕方なく親Objからたどることに。
        canvas.transform.Find("BackGround").gameObject.SetActive(isUse);
        canvas.transform.Find("NowLoading...").gameObject.SetActive(isUse);
        Debug.Log("BaseSceneObj：SetActive設定");
        //backGround.SetActive(isUse);
        //nowLoading.SetActive(isUse);
    }
}