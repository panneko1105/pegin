using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopIce : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rb;
    bool Stopfg;

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
            float sum = Mathf.Abs(rb.velocity.x);
            if (sum < 0.5f)
            {
                StopTime += Time.deltaTime;
                if (StopTime > 0.5f)
                {
                    rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                    Destroy(this);
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
}
