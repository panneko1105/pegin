using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

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

    // ボタンを押した時の処理
    public void Click()
    {
        SceneChangeManager.Instance.SceneChangeOut(SceneChangeType.FADE, 0.5f, m_nextScene);
    }
}
