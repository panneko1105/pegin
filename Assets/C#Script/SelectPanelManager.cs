using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;
using UnityEngine.UI;
using UnityEditor;

public class SelectPanelManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private int    stageNo = 1;                      //!< ステージNo.(1～)
    [SerializeField] private Sprite afterItemTexture;                 //!< 取得後のテクスチャ

    const int itemMax = 3;                                            //!< アイテム合計数
    [SerializeField] GameObject[] itemObj = new GameObject[itemMax];  //!< アイテムObj



    // Start is called before the first frame update
    void Start()
    {
        // 子Objを見つける (Inpectorを使わない場合)
        //Transform obj = this.gameObject.transform.Find("ITEM_STAR");
        //itemObj[0] = obj.Find("Star_1").gameObject;
        //itemObj[1] = obj.Find("Star_2").gameObject;
        //itemObj[2] = obj.Find("Star_3").gameObject;

        // Stage1の2番目を仮に取得状態にする
        GameDataManager.Instance.SaveItemFlg(1, 2);

        // 取得状況に合わせる
        for (int i = 0; i < GameDataManager.Instance.ItemMax; i++)
        {
            // もし取得済みであれば
            if (GameDataManager.Instance.GetItemFlg(stageNo, i + 1))
            {
                // 画像を変更
                Image image = itemObj[i].GetComponent<Image>();
                image.sprite = afterItemTexture;
            }
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
        
    }


    /* ---- ここから拡張コード ---- */
#if UNITY_EDITOR
    /**
     * Inspector拡張クラス
     */
    //[CustomEditor(typeof(SelectPanelManager))]
    //public class SelectPanelManagerEditor : Editor
    //{

    //    public override void OnInspectorGUI()
    //    {
    //        // target は処理コードのインスタンスだよ！ 処理コードの型でキャストして使ってね！
    //        SelectPanelManager selectPanelManager = target as SelectPanelManager;

    //        // -- ステージNo. --
    //        selectPanelManager.stageNo = EditorGUILayout.IntField("ステージNo.", selectPanelManager.stageNo);
    //    }
    //}
#endif
}

