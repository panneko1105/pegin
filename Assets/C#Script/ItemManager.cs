using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour, IUpdatable
{
    const int itemNum = 3;                                               //!< アイテム合計数
    [SerializeField] GameObject[] itemObj = new GameObject[itemNum];     //!< アイテムObj
    [SerializeField] GameObject goalFlgObj;                              //!< ゴールUIObj
    bool[] isGetFlg = new bool[itemNum];                                 //!< 取得の有無 (bool, bool, bool)

    private int stageNo = 1;                                             //!< 現在のステージNo.

    // Start is called before the first frame update
    void Start()
    {
        // 現在どこのステージかの情報を取得
        stageNo = GameDataManager.Instance.GetNowStageNo();

        // 必要分のアイテム情報を用意
        //
        for (int i = 0; i < itemNum; i++)
        {
            // 未取得
            isGetFlg[i] = false;

            // 未取得処理 (保存情報から)
            //if (GameDataManager.Instance.GetItemFlg(stageNo, i + 1))
            //{
            //    // 取得済みにする
            //    FirstItemGetting(i + 1);
            //}

            //=========================
            // Obj生成
            //=========================
            // 生成
            //itemObj[i] = Instantiate(itemPrehfab, new Vector3(40 + i * 40, -30.0f, 0.0f), Quaternion.identity);
            //// 親Objをキャンバスに
            //itemObj[i].transform.SetParent(canvasData.transform, false);
            //// 大きさ調整
            //itemObj[i].transform.localScale = new Vector3(0.4f, 0.4f, 1);
            //// 左上をアンカーとする
            //var rect = itemObj[i].transform.GetComponent<RectTransform>();
            //rect.anchorMin = new Vector2(0.0f, 1.0f);
            //rect.anchorMax = new Vector2(0.0f, 1.0f);
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
        // 関係ないときは動かせない
        if (StageManager.Instance.GetFlg() != StageFlg.NOMAL)
        {
            return;
        }

        //========================================
        // デバッグ用アイテム取得処理　※本番は必ず消すこと！！！！！！！！！！！！！！！！！！！！！！！
        //========================================
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    ItemGetting(1);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    ItemGetting(2);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    ItemGetting(3);
        //}
    }


    //========================================
    // アイテム取得処理 ※1～の指定
    //========================================
    void FirstItemGetting(int num)
    {
        // 例外
        if (num > itemNum)
        {
            return;
        }

        // flg
        isGetFlg[num - 1] = true;

        // 画像切り替え
        Sprite afterPic = Resources.Load<Sprite>("Texture/StarCESA_02");
        Image image = itemObj[num - 1].GetComponent<Image>();
        image.sprite = afterPic;

        // 色切り替え
        //Image image = itemObj[num - 1].GetComponent<Image>();
        image.color = new Color(1.0f, 1.0f, 100.0f / 255.0f, 1.0f);
    }

    public void ItemGetting(int num)
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
            Sprite afterPic = Resources.Load<Sprite>("Texture/StarCESA_02");
            Image image = itemObj[num - 1].GetComponent<Image>();
            image.sprite = afterPic;

            // 色切り替え
            //Image image = itemObj[num - 1].GetComponent<Image>();
            //image.color = new Color(1.0f, 1.0f, 100.0f / 255.0f, 1.0f);

            // アニメーション開始
            StartCoroutine(ItemGettingAnim(num, 0.08f));
            //itemObj[num - 1].transform.localScale = new Vector3(3.0f, 3.0f, 1.0f);

            // 全取得
            if(isGetFlg[0] && isGetFlg[1] && isGetFlg[2])
            {
                // 旗処理
                StartCoroutine(GoalUIAnim(0.1f, 0.12f));

                // SE再生
                SoundManager.Instance.PlaySeEX("magic-cure1");
            }
            else
            {
                // SE再生
                //SoundManager.Instance.PlaySeEX("magic-cure1");
                SoundManager.Instance.PlaySeEX("magic-stick1");
            }
        }
    }

    //========================================
    // アイテム全部取得したかの情報
    //========================================
    public bool GetAllFlg()
    {
        for (int i = 0; i < itemNum; i++)
        {
            // 未取得発見
            if (!isGetFlg[i])
            {
                return false;
            }
        }
        // 全部取得しとるやんけぇ
        return true;
    }

    //========================================
    // アイテム取得アニメーション
    //========================================
    IEnumerator ItemGettingAnim(int num, float seconds)
    {
        //!< 時間計測開始
        float startTime = Time.time;
        float scal = itemObj[num - 1].transform.localScale.x;

        // 
        while (seconds > Time.time - startTime)
        {
            float w = Easing.SineIn(Time.time - startTime, seconds, scal / 100.0f * 200.0f, scal);
            //float w = Easing.BackOut(Time.time - startTime, seconds, scal / 100.0f * 150.0f, scal, 5.0f);
            if (w < 0.0f)
            {
                w = 0.0f;
            }
            itemObj[num - 1].transform.localScale = new Vector3(w, w, 1);

            // 継続
            yield return null;
        }
        // 調整用
        itemObj[num - 1].transform.localScale = new Vector3(scal, scal, 1);
    }

    //========================================
    // ゴールUIが立っちゃうアニメーション///
    //========================================
    IEnumerator GoalUIAnim(float waitSeconds, float seconds)
    {
        yield return new WaitForSecondsRealtime(waitSeconds);

        //!< 時間計測開始
        float startTime = Time.time;
        float scal = goalFlgObj.transform.localScale.x;
        Quaternion rdEX = goalFlgObj.transform.localRotation;
        goalFlgObj.GetComponent<Image>().color = new Color(1.0f, 255.0f / 255.0f, 0.0f / 255.0f, 1.0f);
        //goalFlgObj.GetComponent<Image>().color = Color.yellow;

        // 
        while (seconds > Time.time - startTime)
        {
            float w = Easing.SineIn(Time.time - startTime, seconds, scal / 100.0f * 300.0f, scal);
            float rd = Easing.CubicOut(Time.time - startTime, seconds, rdEX.z * 100.0f, rdEX.z);
            //float w = Easing.BackOut(Time.time - startTime, seconds, scal / 100.0f * 150.0f, scal, 5.0f);
            if (w < 0.0f)
            {
                w = 0.0f;
            }
            goalFlgObj.transform.localScale = new Vector3(w, w, 1);
            goalFlgObj.transform.localRotation = new Quaternion(rdEX.x, rdEX.y, rd, rdEX.w);

            // 継続
            yield return null;
        }
        // 調整用
        goalFlgObj.transform.localScale = new Vector3(scal, scal, 1);
        goalFlgObj.transform.localRotation = rdEX;
    }
}