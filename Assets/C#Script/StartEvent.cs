using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartEvent : MonoBehaviour
{
    bool isStart = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartEventing());
    }

    void Update()
    {

    }

    public IEnumerator StartEventing()
    {
        //!< 時間計測開始
        float seconds = 1.0f;
        ProcessTimer processTimer = new ProcessTimer();
        //!< 色情報取得
        Image image = this.GetComponent<Image>();
        Color col = image.color;

        //=====================================================
        // 右から登場+縦サイズ変更+α値↑
        //=====================================================
        seconds = 0.6f;
        processTimer.Restart();
        while (seconds > processTimer.TotalSeconds)
        {
            // QuadIn
            float x = Easing.BackOut(processTimer.TotalSeconds, seconds, 500.0f, 0.0f, 1.0f);
            float h = Easing.QuartOut(processTimer.TotalSeconds, seconds, 0.3f, 1.0f);
            float a = Easing.QuartOut(processTimer.TotalSeconds, seconds, 0.2f, 1.0f);

            this.transform.localPosition = new Vector3(x, 0, 0);
            this.transform.localScale = new Vector3(1.0f, h, 1.0f);
            image.color = new Color(col.r, col.g, col.b, a);
            // 継続
            yield return null;
        }
        // 調整用
        this.transform.localPosition = new Vector3(0, 0, 0);
        this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        image.color = new Color(col.r, col.g, col.b, 1.0f);

        //=====================================================
        // 待機
        //=====================================================
        yield return new WaitForSecondsRealtime(0.1f);

        //=====================================================
        // サイズ縮小+α値↓
        //=====================================================
        seconds = 0.35f;
        processTimer.Restart();
        while (seconds > processTimer.TotalSeconds)
        {
            // QuadIn
            float wh = Easing.CubicOut(processTimer.TotalSeconds, seconds, 1.0f, 0.6f);
            float a = Easing.SineIn(processTimer.TotalSeconds, seconds, 1.0f, 0.5f);

            this.transform.localScale = new Vector3(wh, wh, 1.0f);
            image.color = new Color(col.r, col.g, col.b, a);

            // 継続
            yield return null;
        }
        // 調整用
        this.transform.localScale = new Vector3(0.6f, 0.6f, 1.0f);
        image.color = new Color(col.r, col.g, col.b, 0.5f);

        //=====================================================
        // サイズ拡大+α値↓
        //=====================================================
        seconds = 0.45f;
        processTimer.Restart();
        while (seconds > processTimer.TotalSeconds)
        {
            // QuadIn
            float wh = Easing.CubicOut(processTimer.TotalSeconds, seconds, 0.6f, 1.6f);
            float a = Easing.SineIn(processTimer.TotalSeconds, seconds, 0.5f, 0.0f);

            this.transform.localScale = new Vector3(wh, wh, 1.0f);
            image.color = new Color(col.r, col.g, col.b, a);

            if (!isStart)
            {
                if(wh >= 1.0f)
                {
                    // 本編動き出す
                    StageManager.Instance.SetFlg(StageFlg.NOMAL);
                    isStart = true;
                }
            }

            // 継続
            yield return null;
        }
        // 調整用
        //this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        //image.color = new Color(col.r, col.g, col.b, 0.0f);

        //Destroy(this.gameObject);
    }
}
