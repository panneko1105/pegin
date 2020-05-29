using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;


public class TheWorld : MonoBehaviour,IUpdatable
{
    private Rigidbody2D rb;

    Vector2 KeepVelcity;
    float KeepAnglu;


    void OnEnable()
    {
        UpdateManager.AddUpdatable(this);
        rb = GetComponent<Rigidbody2D>();
    }

    void OnDisable()
    {
        UpdateManager.RemoveUpdatable(this);
    }

    public void UpdateMe()
    {
        Debug.Log("ここはイケてる");
        if (Input.GetKey(KeyCode.K))
        {
            Debug.Log("謎");
            StopIce();
        }
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
