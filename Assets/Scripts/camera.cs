using UnityEngine;

using UnityEngine;
using System.Collections;

public class camera : MonoBehaviour
{

    public GameObject player;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, 5, -10);
    }
}