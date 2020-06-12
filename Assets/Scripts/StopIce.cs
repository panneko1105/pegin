using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopIce : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rb;
    bool Stopfg;

    private List<float> Point = new List<float>();
    float StopTime;
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        Stopfg = false;
        StopTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Stopfg)
        {
            //一定の速度以下の場合タイムの計測開始
            float sum = Mathf.Abs(rb.velocity.x)+ Mathf.Abs(rb.velocity.y);
            if (sum < 0.5f)
            {
                StopTime += Time.deltaTime;
                if (StopTime > 1.5f)
                {
                    //一定時間とまっていたので固定
                    rb.constraints = RigidbodyConstraints2D.FreezeAll;
                    Stopfg = false;
                    StopTime = 0f;
                }
            }
            else
            {
                //速度が加算されたので再計測
                StopTime = 0f;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "block")
        {
            //地面に落ちた場合を想定
            Stopfg = true;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "block")
        {
            //地面がなくなったとき動き出すため固定を解除
            Stopfg = false;
            rb.constraints = RigidbodyConstraints2D.None;
        }
    }
}
