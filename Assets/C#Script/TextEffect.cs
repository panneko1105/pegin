using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextEffect : MonoBehaviour
{
    [SerializeField] private bool isFadeLoop = false;
    [SerializeField] private float seconds = 2.0f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 0.8f;

    // Use this for initialization
    void Start()
    {
        if (minAlpha < 0.0f)
        {
            minAlpha = 0.0f;
        }
        if(maxAlpha > 1.0f)
        {
            maxAlpha = 1.0f;
        }

        if (isFadeLoop)
        {
            StartCoroutine(StartFadeLoop(seconds, minAlpha, maxAlpha));
        }
    }

    public IEnumerator StartFadeLoop(float _seconds, float _minAlpha = 0.0f, float _maxAlpha = 1.0f)
    {
        // フェードアウト
        yield return StartCoroutine(FadeOut(_seconds, _minAlpha, _maxAlpha));
        // フェードイン
        yield return StartCoroutine(FadeIn(_seconds, _minAlpha, _maxAlpha));

        // ↑の繰り返し
        StartCoroutine(StartFadeLoop(_seconds, _minAlpha, _maxAlpha));
    }

    //========================================
    // フェードアウト [秒] 指定（1.0→0.0）
    //========================================
    public IEnumerator FadeOut(float _seconds, float _minAlpha = 0.0f, float _maxAlpha = 1.0f)
    {
        Debug.Log("Text_フェードアウト開始");

        // 時間計測開始
        float startTime = Time.time;

        //!< 色情報
        float alpha = 1.0f;
        Text text = GetComponent<Text>();
        Color col = text.color;

        while (alpha > _minAlpha)
        {
            // α値どんどん薄くなるよ
            alpha = _maxAlpha - (Time.time - startTime) / _seconds * (_maxAlpha - _minAlpha);
            if (alpha < _minAlpha)
            {
                alpha = _minAlpha;
            }
            text.color = new Color(col.r, col.g, col.b, alpha);

            // 継続
            yield return null;
        }
        Debug.Log("Text_フェードアウト終了");
    }

    //========================================
    // フェードイン [秒] 指定（0.0→1.0）
    //========================================
    public IEnumerator FadeIn(float _seconds, float _minAlpha = 0.0f, float _maxAlpha = 1.0f)
    {
        Debug.Log("Text_フェードイン開始");

        // 時間計測開始
        float startTime = Time.time;

        //!< 色情報
        float alpha = 0.0f;
        Text text = GetComponent<Text>();
        Color col = text.color;

        while (alpha < _maxAlpha)
        {
            // α値どんどん濃くなるよ
            alpha = _minAlpha + (Time.time - startTime) / _seconds * (_maxAlpha - _minAlpha);
            if (alpha > _maxAlpha)
            {
                alpha = _maxAlpha;
            }
            text.color = new Color(col.r, col.g, col.b, alpha);

            // 継続
            yield return null;
        }
        Debug.Log("Text_フェードイン終了");
    }
}
