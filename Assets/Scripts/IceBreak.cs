using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;
using UnityEngine.VFX;

public class IceBreak : MonoBehaviour
{
    float time = 0f;
    void Update()
    {
        time += 1.8f * Time.deltaTime;
        if (time < 3.0f)
        {
            SetParamete(time);
            Debug.Log(time);
        }
        else
        {
            VisualEffect vf = this.GetComponent<VisualEffect>();
            vf.SendEvent("Stop");
        }
    }
    public void SetParamete(float value)
    {
        VisualEffect vfx = this.GetComponent<VisualEffect>();
        vfx.SetFloat("radius", value);
    }
}
