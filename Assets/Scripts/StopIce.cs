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
            float sum = Mathf.Abs(rb.velocity.x)+ Mathf.Abs(rb.velocity.y);
            if (sum < 0.5f)
            {
                StopTime += Time.deltaTime;
                if (StopTime > 1.5f)
                {
                    rb.constraints = RigidbodyConstraints2D.FreezeAll;
                    Stopfg = false;
                    StopTime = 0f;
                }
            }
            else
            {
                StopTime = 0f;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "block")
        {
            Stopfg = true;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "block")
        {
            Stopfg = false;
            rb.constraints = RigidbodyConstraints2D.None;
        }
    }
}
