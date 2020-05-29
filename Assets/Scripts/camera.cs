using UnityEngine;

using UnityEngine;
using System.Collections;

public class camera : MonoBehaviour
{

    public GameObject player;

    // Use this for initialization

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, 5, -27);
    }
}