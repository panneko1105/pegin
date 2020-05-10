using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

// @date 2020/05/06 [今後修正予定]
//
// ボタン押したら指定したInspector上でシーン遷移
//

public class Mybutton : MonoBehaviour, IUpdatable
{
    [SerializeField] private SceneObject m_nextScene;

    // Start is called before the first frame update
    void Start()
    {

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

    }

    //ボタンを押した時の処理
    public void Click()
    {
        //SceneManager.LoadScene(m_nextScene);
        //SceneManager.LoadScene("Stage1");
    }
}
