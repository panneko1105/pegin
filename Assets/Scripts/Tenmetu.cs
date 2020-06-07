using UnityEngine;
using System.Collections;

public class Tenmetu : MonoBehaviour
{
    private float nextTime;
    public float interval = 4f;   // 点滅周期

    // Use this for initialization
    void Start()
    {
        nextTime =0f;
    }

    // Update is called once per frame
    void Update()
    {
        nextTime += 8f * Time.deltaTime;
        if (nextTime>interval)
        {
            var Render = gameObject.GetComponent<Renderer>();
            Render.enabled = !Render.enabled;

            nextTime = 0;
        }
    }
}