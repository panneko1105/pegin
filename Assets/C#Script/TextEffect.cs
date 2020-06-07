using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GokUtil.UpdateManager;

public class TextEffect : MonoBehaviour, IUpdatable
{
    [SerializeField] private bool isFadeLoop = false;
    [SerializeField] private float seconds = 2.0f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 0.8f;
    IEnumerator coroutine;
    IEnumerator parentCoroutine;

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
            StartCoroutine(StartFadeLoop());
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

    public void SetFadeInfo(float _seconds, float _minAlpha = 0.0f, float _maxAlpha = 1.0f)
    {
        //StopFadeCoroutines();

        seconds = _seconds;
        minAlpha = _minAlpha;
        maxAlpha = _maxAlpha;

        //StartCoroutine(StartFadeLoop());
    }

    public IEnumerator StartFadeLoop()
    {
        // フェードアウト
        coroutine = FadeOut();
        yield return StartCoroutine(coroutine);
        // フェードイン
        coroutine = FadeIn();
        yield return StartCoroutine(coroutine);

        // ↑の繰り返し
        parentCoroutine = StartFadeLoop();
        StartCoroutine(parentCoroutine);
    }

    void StopFadeCoroutines()
    {
        Debug.Log("やべぇ");
        StopCoroutine(parentCoroutine);
        parentCoroutine = null;
        Debug.Log("やべぇ");
        StopCoroutine(coroutine);
        coroutine = null;
        Debug.Log("やべぇ");
        // これで済ますのはまずそうだからやめといた
        //StopAllCoroutines();
    }

    //========================================
    // フェードアウト [秒] 指定（1.0→0.0）
    //========================================
    public IEnumerator FadeOut()
    {
        Debug.Log("Text_フェードアウト開始");

        // 時間計測開始
        float startTime = Time.time;

        //!< 色情報
        float alpha = 1.0f;
        Text text = GetComponent<Text>();
        Color col = text.color;

        while (alpha > minAlpha)
        {
            // α値どんどん薄くなるよ
            alpha = maxAlpha - (Time.time - startTime) / seconds * (maxAlpha - minAlpha);
            if (alpha < minAlpha)
            {
                alpha = minAlpha;
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
    public IEnumerator FadeIn()
    {
        Debug.Log("Text_フェードイン開始");

        // 時間計測開始
        float startTime = Time.time;

        //!< 色情報
        float alpha = 0.0f;
        Text text = GetComponent<Text>();
        Color col = text.color;

        while (alpha < maxAlpha)
        {
            // α値どんどん濃くなるよ
            alpha = minAlpha + (Time.time - startTime) / seconds * (maxAlpha - minAlpha);
            if (alpha > maxAlpha)
            {
                alpha = maxAlpha;
            }
            text.color = new Color(col.r, col.g, col.b, alpha);

            // 継続
            yield return null;
        }
        Debug.Log("Text_フェードイン終了");
    }
}
