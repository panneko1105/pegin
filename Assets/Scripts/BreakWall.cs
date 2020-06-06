using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakWall : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        var rb = collision.gameObject.GetComponent<Rigidbody2D>();
        Debug.Log(Mathf.Abs(rb.velocity.x));
        if (Mathf.Abs(rb.velocity.x) > 1f)
        {
            Destroy(this.gameObject);
        }
    }
}
