using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public class StageManager : MonoBehaviour, IUpdatable
{
    public GameObject fadePanel;
    Fade fadeSC;
    bool flg = true;

    // Start is called before the first frame update
    void Start()
    {
        fadeSC = fadePanel.GetComponent<Fade>();
    }

    void OnEnable()
    {
        UpdateManager.AddUpdatable(this);
    }

    void OnDisable()
    {
        UpdateManager.RemoveUpdatable(this);
    }

    // Update is called once per frame
    public void UpdateMe()
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
