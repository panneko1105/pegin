using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class vfx : MonoBehaviour
{
    private VisualEffect visualEffect;
    private float time = 0f;

    // Start is called before the first frame update
    void Start()
    {
        visualEffect = GetComponent<VisualEffect>();
        //visualEffect.SendEvent("MyEvent");
        //visualEffect.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (time > 1.2f)
        {
            visualEffect.Stop();
            Destroy(this.gameObject);
        }
    }
}
