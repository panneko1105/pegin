using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public class CreateFlame : MonoBehaviour, IUpdatable
{
    public Camera maincamera;
    public int DelNum;

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
            GameObject obj = (GameObject)Resources.Load("flame");
            Vector3 Setpos = maincamera.transform.position;
            Setpos.z = 1f;
            obj = Instantiate(obj, Setpos, Quaternion.identity);
            obj.transform.SetParent(this.transform);
        }
    }

    public void DeleteChild()
    {
        if (this.transform.childCount > DelNum)
        {
            var child = transform.GetChild(0);
            //エフェクト発生
            GameObject obj = (GameObject)Resources.Load("icebreak");
            Instantiate(obj, child.transform.position, Quaternion.identity);

            Destroy(child.gameObject);
        }
    }

}