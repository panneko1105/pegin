using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;
using UnityEngine.UI;

public class TitleEffectMove : MonoBehaviour, IUpdatable
{
    [SerializeField] private float speed = 150.0f;
    Image myImage;
    const int maxCnt = 1200;
    int lifeCnt = maxCnt;

    // Start is called before the first frame update
    void Start()
    {
        myImage = GetComponent<Image>();
        speed *= Random.Range(0.8f, 1.2f);
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
        // 上昇
        speed *= 0.9991f;
        transform.position += new Vector3(0, speed * Time.deltaTime, 0);

        // サイズ
        transform.localScale -= new Vector3(0.18f * Time.deltaTime, 0.18f * Time.deltaTime, 0);

        if(transform.localScale.x < 0)
        {
            Destroy(this.gameObject);
            return;
        }

        // 角度
        //transform.localEulerAngles -= new Vector3(0, 0, 20.0f * Time.deltaTime);

        // 減衰
        myImage.color = new Color(0, 210.0f / 255.0f, 1, ((float)lifeCnt / maxCnt) * 0.5f);

        lifeCnt--;

        if (lifeCnt <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
