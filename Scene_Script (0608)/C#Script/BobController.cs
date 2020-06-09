using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobController : MonoBehaviour
{
    //public GameObject target;
    //const float EASING = 0.01f;
    //bool m_startAnimation = false;

    //// Use this for initialization
    //void Start()
    //{

    //}

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(20, 20, 100, 50), "Start"))
    //    {
    //        m_startAnimation = true;
    //    }
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    // ボタンが押されたらアニメーションスタート
    //    if (!m_startAnimation) return;

    //    // 2点間の距離を速度に反映する
    //    Vector3 diff = target.transform.position - transform.position;
    //    Vector3 v = diff * EASING;
    //    transform.position += v;

    //    // 十分近づいたらアニメーション終了
    //    if (diff.magnitude < 0.01f)
    //    {
    //        Debug.Log("END");
    //        m_startAnimation = false;
    //    }
    //}


    /// <summary>
    /// ばねだよ
    /// </summary>

    public GameObject target;
    const float SPRING = 0.05f;     // 0.05f
    const float DAMPING = 0.95f;    // 0.95f
    bool m_startAnimation = false;
    Vector3 v;

    // Use this for initialization
    void Start()
    {
        v = Vector3.zero;
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, 100, 50), "Start"))
        {
            m_startAnimation = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // ボタンが押されたらアニメーションスタート
        if (!m_startAnimation) return;

        // 2点間の距離を速度に反映する
        Vector3 diff = target.transform.position - transform.position;
        v += diff * SPRING;
        v *= DAMPING;
        transform.position += v;

        //十分近づいたらアニメーション終了
        if (Mathf.Abs(v.magnitude) < 0.00005f)
        {
            Debug.Log("END");
            m_startAnimation = false;
        }
    }
}
