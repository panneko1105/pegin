using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private GameObject canvasData;      //!< 親Obj参照データ
    [SerializeField] private GameObject itemPrehfab;     //!< アイテム用Objデータ
    [SerializeField] private Sprite afterItemTexture;    //!< 取得後のテクスチャ
    static int itemNum = 3;                              //!< アイテム合計数

    GameObject[] itemObj;                                //!< アイテムObj
    bool[] isGetFlg;                                     //!< 取得の有無


    // Start is called before the first frame update
    void Start()
    {
        // 必要分のアイテム情報を用意
        isGetFlg = new bool[itemNum];
        itemObj = new GameObject[itemNum];
        for (int i = 0; i < itemNum; i++)
        {
            // 未取得
            isGetFlg[i] = false;

            //=========================
            // Obj生成
            //=========================
            // 生成
            itemObj[i] = Instantiate(itemPrehfab, new Vector3(40 + i * 40, -30.0f, 0.0f), Quaternion.identity);
            // 親Objをキャンバスに
            itemObj[i].transform.SetParent(canvasData.transform, false);
            // 大きさ調整
            itemObj[i].transform.localScale = new Vector3(0.4f, 0.4f, 1);
            // 左上をアンカーとする
            var rect = itemObj[i].transform.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.0f, 1.0f);
            rect.anchorMax = new Vector2(0.0f, 1.0f);
        }

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
        if (num > itemNum)
        {
            return;
        }

        // まだ未取得なら処理
        if (!isGetFlg[num - 1])
        {
            // flg
            isGetFlg[num - 1] = true;

            // 画像切り替え
            Image image = itemObj[num - 1].GetComponent<Image>();
            image.sprite = afterItemTexture;

            // SE再生
            SoundManager.Instance.PlaySe("SE_Test_01");
        }
    }
}