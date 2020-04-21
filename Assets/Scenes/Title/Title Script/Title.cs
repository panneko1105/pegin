using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title: MonoBehaviour
{
   //GameObject test;
    Fade fade;
    bool flg = true;
    // Use this for initialization
    void Start()
    {
        fade = GetComponent<Fade>();
        flg = true;
        fade.StartFadeIn(3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (flg)
        {
            if (Input.GetMouseButton(0))
            {
                flg = false;
                fade.StartFadeOut("main", 3.0f);
            }
        }   
    }
}
