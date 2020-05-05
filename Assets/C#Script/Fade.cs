using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;          // 今後消す、こっから直でシーン遷移させない

// @date 2020/05/01 [今後修正予定]
//
// FadePanelは必要な時にのみ動的生成・削除
// →じゃないと、Buttonが押せない。(FadePanelは一番手前に置くから)
//
// 新しいシーン遷移に対応したい。
//

public class Fade : MonoBehaviour
{
    private float startTime;                //!< 時間計測用
    private float seconds;                  //!< フェードに掛ける時間 [秒]
    private bool isFadeInFlg = false;       //!< フェードインflg
    private bool isFadeOutFlg = false;      //!< フェードアウトflg
    Color color;                            //!< 色設定
    string scene;                           //!< シーン遷移先 (修正予定)


    // Use this for initialization
    void Start()
    {
        startTime = Time.time;        // いらね
    }

    //=========================================================
    // フェードアウト開始
    //=========================================================
    public void StartFadeOut(string _scene, float _seconds)
    {
        isFadeOutFlg = true;
        isFadeInFlg  = false;         // フェードインすなよ(念のため)
        startTime = Time.time;        // 時間計測開始
        scene = _scene;               // フェードアウト終了後のシーン遷移先
        seconds = _seconds;           // フェードに掛ける時間 [秒]
    }

    //=========================================================
    // フェードイン開始
    //=========================================================
    public void StartFadeIn(float _seconds)
    {
        isFadeOutFlg = false;         // フェードアウトすなよ(念のため)
        isFadeInFlg  = true;
        startTime = Time.time;        // 時間計測開始
        seconds = _seconds;           // フェードに掛ける時間 [秒]
    }

    void Update()
    {
        // フェードアウト処理中
        if (isFadeOutFlg)
        {
            // α値が濃くなっていくよ
            color.a = (Time.time - startTime) / seconds;
            GetComponent<Image>().color = new Color(0, 0, 0, color.a);
            //Debug.Log("フェードアウト中");

            // 処理終了
            if (color.a >= 1.0)
            {
                isFadeOutFlg = false;
                // シーン遷移
                SceneManager.LoadScene(scene);
                Debug.Log("フェードアウト終了、シーン遷移先：" + scene);
            }
        }
        //フェードイン処理中
        if (isFadeInFlg)
        {
            // α値の減衰
            color.a = 1.0f - (Time.time - startTime) / seconds;
            GetComponent<Image>().color = new Color(0, 0, 0, color.a);
            //Debug.Log("フェードイン中");

            // 処理終了
            if (color.a <= 0.0)
            {
                isFadeInFlg = false;
                Debug.Log("フェードイン終了");
            }
        }
    }
}