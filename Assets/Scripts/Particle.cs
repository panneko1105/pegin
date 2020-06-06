using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.tag == "block")
        {
            
            if (rb.velocity.y > 0.3f)
            {
                 //エフェクト発生
                GameObject obj = (GameObject)Resources.Load("CFX3_Hit_SmokePuff");
                Instantiate(obj, transform.position, Quaternion.identity);
            }
        }
    }
}
