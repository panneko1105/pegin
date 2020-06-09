using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;
using UnityEngine.UI;
using UnityEditor;

public class SelectPanelManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private int    stageNo = 1;                      //!< ステージNo.(1～)

    const int stageMax = 8;                                           //!< ステージ最大数 (※)
    const int itemMax = 3;                                            //!< アイテム合計数
    [SerializeField] GameObject[] itemObj = new GameObject[itemMax];  //!< アイテムObj
    [SerializeField] GameObject text;                                 //!< STAGE○テキストObj
    [SerializeField] GameObject text2;                                //!< 氷制限数テキストObj

    // Start is called before the first frame update
    void Start()
    {
        // 子Objを見つける (Inpectorを使わない場合)
        //Transform obj = this.gameObject.transform.Find("ITEM_STAR");
        //itemObj[0] = obj.Find("Star_1").gameObject;
        //itemObj[1] = obj.Find("Star_2").gameObject;
        //itemObj[2] = obj.Find("Star_3").gameObject;

        // アイテム情報をセット
        //SetItem();
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

    void SetItem()
    {
        // 取得状況に合わせる
        if (GameDataManager.Instance == null)
        {
            return;
        }

        for (int i = 0; i < GameDataManager.Instance.ItemMax; i++)
        {
            // もし取得済みであれば
            if (GameDataManager.Instance.GetItemFlg(stageNo, i + 1))
            {
                // 画像を変更
                Sprite afterPic = Resources.Load<Sprite>("Texture/StarCESA_02");
                Image image = itemObj[i].GetComponent<Image>();
                image.sprite = afterPic;
            }
        }
    }

    public void SetInfo(int _stageNo)
    {
        if (_stageNo < 1)
        {
            stageNo = 1;
        }
        else if (_stageNo > stageMax) {
            _stageNo = stageMax;
        }

        stageNo = _stageNo;
        // アイテム画像の変更
        //SetItem();

        //「Stage○」テキスト
        Text t = text.GetComponent<Text>();
        t.text = "STAGE " + _stageNo;

        // 氷制限数テキスト
        Text t2 = text2.GetComponent<Text>();
        if (GameDataManager.Instance != null)
        {
            int a = GameDataManager.Instance.GetIceMax(stageNo);
            Debug.Log(a);
            t2.text = a.ToString();
        }
    }

    public IEnumerator MoveAnimation(float seconds)
    {
        //!< 時間計測開始
        float startTime = Time.time;
        float scalX = this.transform.localScale.x;
        float scalY = this.transform.localScale.y;

        while (seconds > Time.time - startTime)
        {
            // QuadIn
            float h = Easing.BackOut(Time.time - startTime, seconds, 0.0f, scalY, 1.0f);
            this.transform.localScale = new Vector3(scalX, h, 0);
            // 継続
            yield return null;
        }
        // 調整用
        this.transform.localScale = new Vector3(scalX, scalY, 0);
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

