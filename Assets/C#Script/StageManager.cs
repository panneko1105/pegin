using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject fadePanel;
    Fade fadeSC;
    bool flg = true;

    // Start is called before the first frame update
    void Start()
    {
        fadeSC = fadePanel.GetComponent<Fade>();
    }

    // Update is called once per frame
    void Update()
    {
        if (flg)
        {
            if (Input.GetMouseButton(0))
            {
                flg = false;
                fadeSC.StartFadeOut("Title", 1.0f);
            }
        }
    }
}
