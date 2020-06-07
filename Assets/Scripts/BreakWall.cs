using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakWall : MonoBehaviour
{
    bool BreakFg = false;
    GameObject KeepBlock;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "block")
        {
            if (KeepBlock == collision.gameObject)
            {
                var rb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (Mathf.Abs(collision.relativeVelocity.x) > 2.5f)
                {
                    Destroy(this.gameObject);
                    rb.velocity = new Vector2(collision.relativeVelocity.x * 0.3f, collision.relativeVelocity.x * 0.3f);
                }
            }
        }

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "block")
        {
            KeepBlock = col.gameObject;
        }
    }
}
