using System.Collections;
using System.Collections.Generic;
<<<<<<< HEAD
using UnityEngine;
using GokUtil.UpdateManager;


public class TheWorld : MonoBehaviour,IUpdatable
=======
using System.Linq;
using UnityEngine;

public class TheWorld : MonoBehaviour
>>>>>>> origin/testing
{
    private Rigidbody2D rb;

    Vector2 KeepVelcity;
    float KeepAnglu;
<<<<<<< HEAD

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
=======
    void Star()
    {
        rb = GetComponent<Rigidbody2D>();
    }
>>>>>>> origin/testing
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
