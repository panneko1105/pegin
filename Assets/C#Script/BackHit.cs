using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackHit : MonoBehaviour
{
    PlayerControl1 P_con;
    public GameObject Player;
    void Start()
    {
        P_con = Player.GetComponent<PlayerControl1>();
    }
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "block")
        {
          
        }
    }
}
