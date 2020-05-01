using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMove : MonoBehaviour
{
    public float speed = 3.0f;
    void Start()
    {
       
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}