using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

// @date 2020/05/01 [今後修正予定]
//
// アイテムObj (☆)は初めから表示
// →取得したら別の画像 (★) に変更。なのでInstantiateは行わなくする。
//

public class ItemManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private GameObject canvasData;      //!< 親Obj参照データ
    [SerializeField] private GameObject starPrehfab;     //!< アイテム用Objデータ
    int itemNum = 3;                                     //!< アイテム合計数

    GameObject[] star;                                   //!< アイテムObj
    bool[] isGetFlg;                                     //!< 取得の有無


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
        // もしポーズ中なら処理しない
        if (PauseManager.Instance.GetisPause())
        {
            return;
        }

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
            star[num - 1] = Instantiate(starPrehfab, new Vector3(40 + (num - 1) * 40, -30.0f, 0.0f), Quaternion.identity);
            // 親Objをキャンバスに
            star[num - 1].transform.SetParent(canvasData.transform, false);
            // 大きさ調整
            star[num - 1].transform.localScale = new Vector3(0.4f, 0.4f, 1);
            // 左上をアンカーとする
            var rect = star[num - 1].transform.GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(0.0f, 1.0f);
            rect.anchorMin = new Vector2(0.0f, 1.0f);

            // SE再生
            SoundManager.Instance.PlaySe("SE_Test_01");
        }
    }
}