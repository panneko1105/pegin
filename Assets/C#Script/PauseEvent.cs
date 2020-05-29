using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator AnimLeftBlue(float seconds)
    {
        //!< 時間計測開始
        ProcessTimer processTimer = new ProcessTimer();
        processTimer.Restart();

        //!< 保存用
        Vector3 pos = this.gameObject.transform.position;
        //!< ラジアンに変換 (3.0度→)
        float rad = 3.0f * Mathf.Deg2Rad;

        while (seconds > processTimer.TotalSeconds)
        {
            float move = Easing.CubicOut(processTimer.TotalSeconds, seconds, -1100, 0.0f);
            this.gameObject.transform.localPosition = new Vector3(move * Mathf.Sin(rad), move * Mathf.Cos(rad), 0);

            // 継続
            yield return null;
        }

        // 調整用
        this.gameObject.transform.position = pos;
    }
}
