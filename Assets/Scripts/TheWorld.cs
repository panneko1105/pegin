using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TheWorld : MonoBehaviour
{
    private Rigidbody2D rb;

    Vector2 KeepVelcity;
    float KeepAnglu;
    void Star()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void StopIce()
    {
        KeepVelcity = rb.velocity;
        KeepAnglu = rb.angularVelocity;
        rb.Sleep();

        Debug.Log("aaaaaaaaaa");
    }

    public void StartIce()
    {
        rb.WakeUp();
        rb.velocity = KeepVelcity;
        rb.angularVelocity = KeepAnglu;

        Debug.Log("bbbbbbbbbbbb");
    }
}
