using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakWall : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "block")
        {
            var rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (Mathf.Abs(collision.relativeVelocity.x) > 1f)
            {
                Destroy(this.gameObject);
                rb.velocity = new Vector2(collision.relativeVelocity.x * 0.4f, collision.relativeVelocity.x * -0.4f);
            }
        }

    }
}
