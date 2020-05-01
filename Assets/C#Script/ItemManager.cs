using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public class ItemManager : MonoBehaviour, IUpdatable
{
    public GameObject canvasData;      //!< 親Obj参照データ
    public GameObject starPrehfab;      //!< アイテム用Objデータ
    public int itemNum = 3;            //!< アイテム合計数

    GameObject[] star;                   //!< アイテムObj
    bool[] isGetFlg;                   //!< 取得の有無

    void OnEnable()
    {
        UpdateManager.AddUpdatable(this);
    }

    void OnDisable()
    {
        UpdateManager.RemoveUpdatable(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        // 必要分のアイテム情報を用意
        isGetFlg = new bool[itemNum];
        for(int i = 0; i < itemNum; i++)
        {
            isGetFlg[i] = false;
        }
        // 必要数分Objを用意
        star = new GameObject[itemNum];
    }

    // Update is called once per frame
    public void UpdateMe()
    {
        //========================================
        // デバッグ用アイテム取得処理
        //========================================
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetItem(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetItem(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetItem(3);
        }
    }


    //========================================
    // アイテム取得処理 ※1～の指定
    //========================================
    public void SetItem(int num)
    {
        // 例外
        if(num > itemNum)
        {
            return;
        }

        // まだ未取得なら処理
        if (!isGetFlg[num - 1])
        {
            isGetFlg[num - 1] = true;
            // 生成
            star[num - 1] = Instantiate(starPrehfab, new Vector3(-400.0f + (num - 1) * 60, 160.0f, 0.0f), Quaternion.identity);
            star[num - 1].transform.SetParent(canvasData.transform, false);
            star[num - 1].transform.localScale = new Vector3(0.5f, 0.5f, 1);
        }
    }
}
