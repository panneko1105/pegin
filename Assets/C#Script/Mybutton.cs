﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @date 2020/05/01 [今後修正予定]
//
// ボタン押したら指定したInspector上でシーン遷移
//

public class Mybutton : MonoBehaviour
{
    [SerializeField] private SceneObject m_nextScene;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //ボタンを押した時の処理
    public void Click()
    {
        //SceneManager.LoadScene(m_nextScene);
        //SceneManager.LoadScene("Stage1");
    }
}
