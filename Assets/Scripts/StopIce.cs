using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopIce : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rb;
    bool Stopfg;
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        Stopfg = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Stopfg)
        {
            float sum = Mathf.Abs(rb.velocity.x);// + Mathf.Abs(rb.velocity.y);
            if (sum < 0.01f)
            {
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                Destroy(this);
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
