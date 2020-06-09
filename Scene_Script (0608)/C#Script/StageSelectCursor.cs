using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public class StageSelectCursor : MonoBehaviour, IUpdatable
{
    // Start is called before the first frame update
    void Start()
    {
        
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

    public IEnumerator MoveAnimation(float seconds, Vector3 firstPos, Vector3 lastPos)
    {
        Debug.Log("カーソル移動開始");

        //!< 時間計測開始
        float startTime = Time.time;

        // 必要な移動量を求める
        //float moveX = lastPos.x - firstPos.x;
        //float moveY = lastPos.y - firstPos.y;

        while (seconds > Time.time - startTime)
        {
            // QuadIn
            float x = Easing.SineIn(Time.time - startTime, seconds, firstPos.x, lastPos.x);
            float y = Easing.SineIn(Time.time - startTime, seconds, firstPos.y, lastPos.y);
            this.transform.position = new Vector3(x, y, 0);
            // 継続
            yield return null;
        }
        // 調整用
        this.transform.position = new Vector3(lastPos.x, lastPos.y, 0);

        Debug.Log("カーソル移動終了");
    }
}
