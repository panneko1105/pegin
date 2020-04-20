using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    float startTime;                //!< 時間計測用
    float seconds;                  //!< フェードに掛ける時間 [秒]
    Color color;                    //!< 色設定
    bool isFadeInFlg = false;       //!< フェードインflg
    bool isFadeOutFlg = false;      //!< フェードアウトflg
    string scene;                   //!< シーン遷移先


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