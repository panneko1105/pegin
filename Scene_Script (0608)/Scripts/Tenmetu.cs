using UnityEngine;
using System.Collections;


public class Tenmetu : MonoBehaviour
{
    private float nextTime;
    public float interval = 4f;   // 点滅周期
    Renderer Render;

    // Use this for initialization
    void Start()
    {
        nextTime =0f;
        Render= gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        nextTime += 8f * Time.deltaTime;
        if (nextTime>interval)
        {
            Render.enabled = !Render.enabled;

            nextTime = 0;
        }
    }

    public void Cancel()
    {
        nextTime = 0;
        Render.enabled = true;
    }
}