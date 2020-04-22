using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    //GameObject findObjTest;
    public GameObject canvasData;       //!< 親Obj参照データ
    public GameObject textPrefab;       //!< 文字Objのプレハブ
    public GameObject cursorPrefab;     //!< カーソル用Objのプレハブ

    //GameObject obj;
    GameObject[] msg;                   //!< 文字列obj
    bool isPause = false;               //!< ポーズflg
    const int selectNum = 3;            //!< 選択肢数
    string[] message = new string[]     //!< 選択肢ワード
    {
        "再開",
        "リスタート",
        "終了",
    };
    int selectPos = 0;                  //!< 0が一番上の選択肢位置
    GameObject cursor;

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

    // Update is called once per frame
    void Update()
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

                switch (selectPos) {
                    case 0:
                        // このまま閉じる
                        break;
                    case 1:
                        // リスタート (仮にシーン読込直し)
                        SceneManager.LoadScene("Stage1");
                        break;
                    case 2:
                        // ゲーム終了
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
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
                // カーソル作成
                cursor = Instantiate(cursorPrefab, new Vector3(0.0f, ((selectNum - 1) / 2.0f) * 60 - selectPos * 60, 0.0f), Quaternion.identity);
                cursor.transform.SetParent(canvasData.transform, false);
                cursor.transform.localScale = new Vector3(3.0f, 0.6f, 1);

                // メッセージ生成
                for (int i = 0; i < selectNum; i++)
                {
                    msg[i] = Instantiate(textPrefab, new Vector3(0.0f, ((selectNum - 1) / 2.0f) * 60 - i * 60, 0.0f), Quaternion.identity);
                    msg[i].transform.SetParent(canvasData.transform, false);
                    msg[i].transform.localScale = new Vector3(0.20f, 0.16f, 1);
                    Text t = msg[i].GetComponent<Text>();
                    t.text = message[i];
                }
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
        Destroy(cursor);
    }
}
