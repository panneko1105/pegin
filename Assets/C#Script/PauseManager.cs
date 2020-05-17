using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GokUtil.UpdateManager;

public class PauseManager : SingletonMonoBehaviour<PauseManager>, IUpdatable
{
    //GameObject findObjTest;
    [SerializeField] private GameObject canvasData;       //!< 親Obj参照データ
    [SerializeField] private GameObject textPrefab;       //!< 文字Objのプレハブ
    [SerializeField] private GameObject cursorPrefab;     //!< カーソル用Objのプレハブ

    /// <summary>
    /// いらなくなるかも
    /// </summary>
    GameObject back;
    GameObject[] msg;                   //!< 文字列用obj
    bool isPause = false;               //!< ポーズflg
    const int selectNum = 3;            //!< 選択肢数
    string[] message = new string[]     //!< 選択肢ワード
    {
        "ゲームに戻る",
        "リトライ",
        "セレクト画面に戻る",
    };
    int selectPos = 0;                  //!< 0が一番上の選択肢位置
    GameObject cursor;                  //!< カーソル用Obj

    GameObject pausePanel;

    // Start is called before the first frame update
    void Start()
    {
        // 必要数分Objを用意
        msg = new GameObject[selectNum];
        //findObjTest = (GameObject)Resources.Load("Image");
        //canvas = GameObject.Find("Canvas");

        //// 文字生成方法test
        //// １：Instantiateで生成するObject、座標を指定
        //obj[0] = Instantiate(textTest, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
        //// ２：親をキャンバスに設定 (UIにするため)
        //obj[0].transform.SetParent(canvas.transform, false);
        //// ３：Textを取得して文字列の変更
        //Text t = obj[0].GetComponent<Text>();
        //t.text = "こんにちは";

        //obj= Instantiate(Panel, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
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
        // メニュー更新
        if (isPause)
        {

            //========================================
            // カーソル移動
            //========================================
            // カーソルを１つ上へ
            if (Input.GetKeyDown(KeyCode.W))
            {
                selectPos--;
                // 上端きたら下端へ
                if (selectPos < 0)
                {
                    selectPos = selectNum - 1;
                }
                cursor.transform.localPosition = new Vector3(0.0f, ((selectNum - 1) / 2.0f) * 60 - selectPos * 60, 0.0f);
            }
            // カーソルを１つ下へ
            if (Input.GetKeyDown(KeyCode.S))
            {
                selectPos++;
                // 下端きたら上端へ
                if (selectPos >= selectNum)
                {
                    selectPos = 0;
                }
                cursor.transform.localPosition = new Vector3(0.0f, ((selectNum - 1) / 2.0f) * 60 - selectPos * 60, 0.0f);
            }


            //========================================
            // 選択
            //========================================
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ClosePauseMenu();

                switch (selectPos)
                {
                    case 0:
                        // このまま閉じる
                        break;
                    case 1:
                        // リスタート (同じシーンを読み込み直す)
                        LoadingScene.Instance.LoadScene(LoadingScene.Instance.GetNowScene());
                        break;
                    case 2:
                        // セレクト画面へ
                        LoadingScene.Instance.LoadScene("StageSelect");
                        //#if UNITY_EDITOR
                        //                        UnityEditor.EditorApplication.isPlaying = false;
                        //#elif UNITY_STANDALONE
                        //      UnityEngine.Application.Quit();
                        //#endif
                        break;
                    default:
                        // ありえへん
                        Debug.Log("Select_ERROR");
                        break;
                }
            }
            // Escapeでとりあえず閉じる
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClosePauseMenu();
            }
        }
        // メニュー開いてないよ
        else
        {
            // 開くよ
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isPause = true;
                // ザ・ワールド
                Time.timeScale = 0f;

                // もしポーズするたびに選択カーソル位置を一番上にリセットするならコレ
                selectPos = 0;

                // 白背景
                back = new GameObject("BACK_WHITE");
                back.transform.SetParent(canvasData.transform, false);
                back.transform.localScale = new Vector3(8.0f, 3.5f, 1);
                Image image = back.AddComponent<Image>();
                image.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
                var rect = back.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.0f, 0.0f);
                rect.anchorMax = new Vector2(1.0f, 1.0f);
                rect.offsetMin = new Vector2(0.0f, 0.0f); // left, bottom
                rect.offsetMax = new Vector2(0.0f, 0.0f); // right, up
                

                // カーソル作成
                cursor = Instantiate(cursorPrefab, new Vector3(0.0f, ((selectNum - 1) / 2.0f) * 60 - selectPos * 60, 0.0f), Quaternion.identity);
                cursor.transform.SetParent(canvasData.transform, false);
                cursor.transform.localScale = new Vector3(3.0f, 0.6f, 1);
                rect = cursor.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.0f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);

                // メッセージ生成
                for (int i = 0; i < selectNum; i++)
                {
                    msg[i] = Instantiate(textPrefab, new Vector3(0.0f, ((selectNum - 1) / 2.0f) * 60 - i * 60, 0.0f), Quaternion.identity);
                    msg[i].transform.SetParent(canvasData.transform, false);
                    msg[i].transform.localScale = new Vector3(0.18f, 0.14f, 1);
                    var rectin = msg[i].GetComponent<RectTransform>();
                    rectin.anchorMin = new Vector2(0.0f, 0.5f);
                    rectin.anchorMax = new Vector2(0.5f, 0.5f);
                    Text t = msg[i].GetComponent<Text>();
                    t.text = message[i];
                    t.color = Color.black;
                }

                GameObject obj = (GameObject)Resources.Load("ALL PANEL");
                // Cubeプレハブを元に、インスタンスを生成、
                pausePanel = Instantiate(obj, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
                pausePanel.transform.SetParent(canvasData.transform, false);
                Animation anim = obj.GetComponent<Animation>();
                anim.Play();
            }
        }
    }

    void ClosePauseMenu()
    {
        isPause = false;
        // 通常営業
        Time.timeScale = 1f;
        // メッセージ削除
        for (int i = 0; i < selectNum; i++)
        {
            Destroy(msg[i]);
        }
        // カーソル削除
        Destroy(cursor);
        // 白背景削除
        Destroy(back);

        // 選択パネルの削除
        Destroy(pausePanel);
    }

    public bool GetisPause()
    {
        return isPause;
    }
}
