using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameMaskMove : MonoBehaviour
{
    [HideInInspector] public float move = 0.05f;
    GameObject flame;
    FlameMove script;
    // Start is called before the first frame update
    void Start()
    {
        flame = GameObject.Find("Flame");
        
        script= flame.GetComponent<FlameMove>();    // フレームのscriptを取得
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 worldAngle = flame.transform.eulerAngles;   // 度数法

       // flameMask(自身)のサイズを取得
       Transform trans = this.transform;
        Vector3 myScal = trans.lossyScale;
        // flameのサイズを取得
        Transform trans2 = flame.transform;
        Vector3 flameScal = trans2.lossyScale;
        // 初期化
        move = myScal.y / 2.0f;
        //flameが傾きいるかどうか
        if (script.isRot)
        {
            // 中心から角までの距離
            var sin = flameScal.x * Mathf.Sin(45 * (Mathf.PI / 180));
            switch (script.flg)
            {
                case 0:
                    move -= sin / 2.0f;
                    break;
                case 1:
                    //move += 0.0f;
                    break;
                case 2:
                    move += sin / 2.0f;
                    break;
                case 3:
                    move += sin;
                    break;
            }
        }
        // 正常位
        else
        {
            switch (script.flg)
            {
                case 0:
                    move -= flameScal.y / 4.0f;
                    break;
                case 1:
                    //move += 0.0f;
                    break;
                case 2:
                    move += flameScal.y / 4.0f;
                    break;
                case 3:
                    move += flameScal.y / 2.0f;
                    break;
            }
        }

       // Debug.Log(move);  // Console 表示
        this.transform.position = flame.transform.position;
        this.transform.position += new Vector3(0, move, 0);
    }
}