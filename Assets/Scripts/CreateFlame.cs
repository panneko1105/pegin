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
            obj.tag = "flame";

            GameObject obj2 = (GameObject)Resources.Load("maskBox");
            Vector3 Setpos2 = maincamera.transform.position;
            Setpos2.z = -1f;
            Setpos2.y += 1f;
            obj2 = Instantiate(obj2, Setpos2, Quaternion.identity);
            obj2.transform.SetParent(this.transform);

        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            for (int i = 0; i < this.transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.tag != "block")
                {
                    if (child.transform.GetChild(0).tag == "flame")
                    {
                        var sc = child.GetComponent<FlameMove>();
                        sc.CreateIce();
                    }
                    else
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
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