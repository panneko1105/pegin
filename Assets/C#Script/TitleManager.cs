using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    public GameObject fadePanel;
    Fade fadeSC;
    bool flg = true;

    // Use this for initialization
    void Start()
    {
        fadeSC = fadePanel.GetComponent<Fade>();
        flg = true;
        fadeSC.StartFadeIn(1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (flg)
        {
            if (Input.GetMouseButton(0))
            {
                flg = false;
                fadeSC.StartFadeOut("Stage1", 1.0f);
            }
        }   
    }
}
