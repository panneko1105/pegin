using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title: MonoBehaviour
{
    public GameObject FadePanel;//追加
    Fade fadeSC;
    bool flg = true;
    // Use this for initialization
    void Start()
    {
        fadeSC = FadePanel.GetComponent<Fade>();
        flg = true;
        fadeSC.StartFadeIn(3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (flg)
        {
            if (Input.GetMouseButton(0))
            {
                flg = false;
                fadeSC.StartFadeOut("main", 3.0f);
            }
        }   
    }
}
