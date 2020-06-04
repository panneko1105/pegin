using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    //================================================
    //  上下
    //================================================
    public IEnumerator AnimUpdown(float seconds, float firstPosY)
    {
        //!< 時間計測開始
        ProcessTimer processTimer = new ProcessTimer();
        processTimer.Restart();

        //!< 保存用
        Vector3 pos = this.gameObject.transform.localPosition;
        //!< ラジアンに変換 (3.0度→)
        float rad = 3.0f * Mathf.Deg2Rad;

        while (seconds > processTimer.TotalSeconds)
        {
            float move = Easing.CubicOut(processTimer.TotalSeconds, seconds, firstPosY, 0.0f);
            //Debug.Log(move * Mathf.Sin(rad));
            this.gameObject.transform.localPosition = new Vector3(pos.x - move * Mathf.Sin(rad), move * Mathf.Cos(rad), 0);

            // 継続
            yield return null;
        }

        // 調整用
        this.gameObject.transform.localPosition = pos;
    }

    //================================================
    //  左右
    //================================================
    public IEnumerator AnimLeftRight(float waitSeconds, float seconds, float distance)
    {
        //!< 保存用
        Vector3 pos = this.gameObject.transform.localPosition;
        //!< ラジアンに変換 (3.0度→)
        float rad = 3.0f * Mathf.Deg2Rad;

        // 待機
        float move = Easing.CubicOut(0.0f, seconds, distance, 0.0f);
        //Debug.Log(move * Mathf.Sin(rad));
        this.gameObject.transform.localPosition = new Vector3(pos.x + move * Mathf.Cos(rad), pos.y + move * Mathf.Sin(rad), 0);
        yield return new WaitForSecondsRealtime(waitSeconds);

        //!< 時間計測開始
        ProcessTimer processTimer = new ProcessTimer();
        processTimer.Restart();

        while (seconds > processTimer.TotalSeconds)
        {
            move = Easing.QuintOut(processTimer.TotalSeconds, seconds, distance, 0.0f);
            //Debug.Log(move * Mathf.Sin(rad));
            this.gameObject.transform.localPosition = new Vector3(pos.x + move * Mathf.Cos(rad), pos.y + move * Mathf.Sin(rad), 0);

            // 継続
            yield return null;
        }

        // 調整用
        this.gameObject.transform.localPosition = pos;
    }

    public IEnumerator AnimLeftRight2(float waitSeconds, float seconds, float distance)
    {
        //!< 保存用
        Vector3 pos = this.gameObject.transform.localPosition;
        //!< ラジアンに変換 (3.0度→)
        float rad = 3.0f * Mathf.Deg2Rad;

        // 待機
        float move = Easing.CubicOut(0.0f, seconds, distance, 0.0f);
        //Debug.Log(move * Mathf.Sin(rad));
        this.gameObject.transform.localPosition = new Vector3(pos.x + move * Mathf.Cos(rad), pos.y + move * Mathf.Sin(rad), 0);
        yield return new WaitForSecondsRealtime(waitSeconds);

        //!< 時間計測開始
        ProcessTimer processTimer = new ProcessTimer();
        processTimer.Restart();

        while (seconds > processTimer.TotalSeconds)
        {
            move = Easing.CircIn(processTimer.TotalSeconds, seconds, distance, 0.0f);
            //Debug.Log(move * Mathf.Sin(rad));
            this.gameObject.transform.localPosition = new Vector3(pos.x + move * Mathf.Cos(rad), pos.y + move * Mathf.Sin(rad), 0);

            // 継続
            yield return null;
        }

        // 調整用
        this.gameObject.transform.localPosition = pos;
    }
}
