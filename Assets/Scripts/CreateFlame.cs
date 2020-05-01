using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public class CreateFlame : MonoBehaviour, IUpdatable
{
    void OnEnable()
    {
        UpdateManager.AddUpdatable(this);
    }

    void OnDisable()
    {
        UpdateManager.RemoveUpdatable(this);
    }

    // Use this for initialization
    public void UpdateMe()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // CubeプレハブをGameObject型で取得
            GameObject obj = (GameObject)Resources.Load("flame");
            // Cubeプレハブを元に、インスタンスを生成、
            Instantiate(obj, new Vector3(-0.5f, -1.0f, 1.0f), Quaternion.identity);
        }
    }

}