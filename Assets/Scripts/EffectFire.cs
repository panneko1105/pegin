using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

public class EffectFire : MonoBehaviour
{
    public UnityEngine.VFX.VisualEffect effect;
   
    //VisualEffect変数を設定。

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            effect.SendEvent("OnPlay");
            if (effect.GetFloat("maxsize") > 6.0f)
            {

            }
            {
                //  effect.Play();
                Debug.Log("押せてるぞ");
            }
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            effect.SendEvent("Stop");
        }
    }

}