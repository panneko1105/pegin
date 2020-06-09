using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using GokUtil.UpdateManager;

public class FadeManager : MonoBehaviour
{
    [SerializeField] private float seconds = 2.0f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 0.8f;
    private IEnumerator coroutine;
    private IEnumerator parentCoroutine;

    // Use this for initialization
    void Start()
    {
        //if (isFadeLoop)
        //{
        //    StartCoroutine(StartFadeLoop(seconds, minAlpha));
        //}
    }

    public void SetFadeInfo(float _seconds, float _minAlpha = 0.0f, float _maxAlpha = 1.0f)
    {
        seconds = _seconds;
        minAlpha = _minAlpha;
        maxAlpha = _maxAlpha;
    }

    public IEnumerator StartFadeLoop()
    {
        // フェードアウト
        coroutine = FadeOut(seconds, minAlpha, maxAlpha);
        yield return StartCoroutine(coroutine);

        // フェードイン
        coroutine = FadeIn(seconds, minAlpha, maxAlpha);
        yield return StartCoroutine(coroutine);

        // ↑の繰り返し
        parentCoroutine = StartFadeLoop();
        StartCoroutine(parentCoroutine);
    }

    //========================================
    // フェードアウト [秒] 指定（1.0→0.0）
    //========================================
    public IEnumerator FadeOut(float _seconds, float _minAlpha = 0.0f, float _maxAlpha = 1.0f)
    {
        Debug.Log("フェードアウト開始");

        // 時間計測開始
        float startTime = Time.time;

        //!< 色情報
        float alpha = 1.0f;
        Image image = GetComponent<Image>();
        Color col = image.color;

        while (alpha > minAlpha)
        {
            // α値どんどん薄くなるよ
            alpha = _maxAlpha - (Time.time - startTime) / _seconds * (_maxAlpha - _minAlpha);
            if (alpha < _minAlpha)
            {
                alpha = _minAlpha;
            }
            image.color = new Color(col.r, col.g, col.b, alpha);

            // 継続
            yield return null;
        }
        Debug.Log("フェードアウト終了");
    }

    //========================================
    // フェードイン [秒] 指定（0.0→1.0）
    //========================================
    public IEnumerator FadeIn(float _seconds, float _minAlpha = 0.0f, float _maxAlpha = 1.0f)
    {
        Debug.Log("フェードイン開始");

        // 時間計測開始
        float startTime = Time.time;

        //!< 色情報
        float alpha = 0.0f;
        Image image = GetComponent<Image>();
        Color col = image.color;

        while (alpha < maxAlpha)
        {
            // α値どんどん濃くなるよ
            alpha = _minAlpha + (Time.time - startTime) / _seconds * (_maxAlpha - _minAlpha);
            if (alpha > _maxAlpha)
            {
                alpha = _maxAlpha;
            }
            image.color = new Color(col.r, col.g, col.b, alpha);

            // 継続
            yield return null;
        }
        Debug.Log("フェードイン終了");
    }

    //========================================
    // シーン用フェードアウト [秒] 指定
    //========================================
    public IEnumerator SceneFadeOut(float seconds)
    {
        // 時間計測開始
        float startTime = Time.time;
   
        //!< 色情報
        float alpha = 0.0f;
        Image image = GetComponent<Image>();
        Color col = image.color;

        while (alpha < 1.0f)
        {
            // α値どんどん濃くなるよ
            alpha = (Time.time - startTime) / seconds;
            if(alpha > 1.0f)
            {
                alpha = 1.0f;
            }
            image.color = new Color(col.r, col.g, col.b, alpha);

            // 継続
            yield return null;
        }
        Debug.Log("フェードイン終了");
    }

    //========================================
    // シーン用フェードイン [秒] 指定
    //========================================
    public IEnumerator SceneFadeIn(float seconds)
    {
        // 時間計測開始
        ProcessTimer processTimer = new ProcessTimer();
        processTimer.Restart();

        //!< 色情報
        float alpha = 1.0f;
        Image image = GetComponent<Image>();
        Color col = image.color;

        while (alpha > 0.0f)
        {
            // α値どんどん薄くなるよ
            alpha = 1.0f - processTimer.TotalSeconds / seconds;
            if (alpha < 0.0f)
            {
                alpha = 0.0f;
            }
            image.color = new Color(0, 0, 0, alpha);

            // 継続
            yield return null;
        }
        Debug.Log("シーン_フェードイン終了");
        // いらねぇ
        Destroy(this.gameObject);
    }

    public void StopFadeCoroutines()
    {
        StopCoroutine(coroutine);
        StopCoroutine(parentCoroutine);
        // これで済ますのはまずそうだからやめといた
        //StopAllCoroutines();
    }
}