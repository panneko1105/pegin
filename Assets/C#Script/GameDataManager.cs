using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : SingletonMonoBehaviour<GameDataManager>
{
    private static int stageMax = 8;                                //!< ステージ最大数 (固定、ここ以外で弄れない)
    private static int itemMax = 3;                                 //!< １ステージのアイテム最大数 (固定、ここ以外で弄れない)
    private bool[,] isItemGetFlg = new bool[stageMax, itemMax];     //!< 取得の有無 (今後ステージ分これ用意したい)
    private int nowStageNo = 1;                                     //!< 現在のステージNo.
    private int stageSelectPos = 1;                                 //!< ステージセレクトのカーソル位置保存
    private int[] stageIceMax = new int[stageMax];                  //!< 各ステージの氷制限数
    int[] iceYeah = new int[]                                       //!< 氷制限数設定
    {
        1, 1, 1, 1,
        1, 3, 5, 99,
    };

    // Start is called before the first frame update
    void Start()
    {
        // 配列の初期化
        for (int i = 0; i < isItemGetFlg.GetLength(0); i++)
        {
            for (int j = 0; j < isItemGetFlg.GetLength(1); j++)
            {
                // アイテムは全て未取得からスタート
                isItemGetFlg[i, j] = false;
            }
        }

        // 氷制限初期化
        for(int i = 0; i < stageMax; i++)
        {
            stageIceMax[i] = iceYeah[i];
        }
    }

    //====================================================================
    // アイテム情報の保存
    // (※ステージNo.、何番目のアイテムか)
    // (※_stageNo.は1～, _itemNoも1～の指定)
    //====================================================================
    public void SaveItemFlg(int _stageNo, int _itemNo)
    {
        // ステージ指定_例外防止
        if (_stageNo < 1)
        {
            _stageNo = 1;
        }
        else if (_stageNo > stageMax)
        {
            _stageNo = stageMax;
        }

        // アイテム指定_例外防止
        if (_itemNo < 1)
        {
            _itemNo = 1;
        }
        else if (_itemNo > itemMax)
        {
            _itemNo = itemMax;
        }

        // 取得扱いに
        isItemGetFlg[_stageNo - 1, _itemNo - 1] = true;
    }

    //====================================================================
    // アイテム情報の取得
    // (※ステージNo.、何番目のアイテムか)
    // (※_stageNo.は1～, _itemNoも1～の指定)
    //====================================================================
    public bool GetItemFlg(int _stageNo, int _itemNo)
    {
        // ステージ指定_例外防止
        if (_stageNo < 1)
        {
            _stageNo = 1;
        }
        else if (_stageNo > stageMax)
        {
            _stageNo = stageMax;
        }

        // アイテム指定_例外防止
        if (_itemNo < 1)
        {
            _itemNo = 1;
        }
        else if (_itemNo > itemMax)
        {
            _itemNo = itemMax;
        }

        return isItemGetFlg[_stageNo - 1, _itemNo - 1];
    }

    //====================================================================
    // アイテムの合計数取得
    //====================================================================
    public int GetItemAllNum()
    {
        int itemNum = 0;

        for (int i = 0; i < isItemGetFlg.GetLength(0); i++)
        {
            for (int j = 0; j < isItemGetFlg.GetLength(1); j++)
            {
                // 取得済
                if (isItemGetFlg[i, j])
                {
                    itemNum++;
                }
            }
        }

        // trueの数
        return itemNum;
    }

    //====================================================================
    // ステージ最大数設定の取得
    //====================================================================
    public int StageMax {
        get { return stageMax; }
    }

    //====================================================================
    // アイテム最大数設定の取得
    //====================================================================
    public int ItemMax
    {
        get { return itemMax; }
    }

    public int StageSelectPos
    {
        get { return stageSelectPos; }
        set { stageSelectPos = value; }
    }


    //====================================================================
    // 現在のステージNo.を更新
    //====================================================================
    public void SetNowStageNo(int _stageNo)
    {
        if (_stageNo < 1)
        {
            _stageNo = 1;
        }
        else if(_stageNo> stageMax)
        {
            _stageNo = stageMax;
        }

        nowStageNo = _stageNo;
    }

    //====================================================================
    // 現在のステージNo.を取得
    //====================================================================
    public int GetNowStageNo()
    {
        return nowStageNo;
    }

    //====================================================================
    // 氷制限数を取得
    //====================================================================
    public int GetIceMax(int _stageNo)
    {
        if (_stageNo < 1)
        {
            _stageNo = 1;
        }
        else if (_stageNo > stageMax)
        {
            _stageNo = stageMax;
        }

        return stageIceMax[_stageNo - 1];
    }
}