using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public class CreateFlame : MonoBehaviour, IUpdatable
{
    public Camera maincamera;
    public int DelNum;
    private bool SpownMode = false;         //trueで出ている状態

    public GameObject Player;
    public GameObject Camera;
    PlayerControl1 WalkCon;
    Post StopMono;
    bool OnceMove;


    void OnEnable()
    {
        UpdateManager.AddUpdatable(this);
        WalkCon = Player.GetComponent<PlayerControl1>();
        OnceMove = true;
        StopMono = Camera.GetComponent<Post>();
    }

    void OnDisable()
    {
        UpdateManager.RemoveUpdatable(this);
    }

    // Use this for initialization
    public void UpdateMe()
    {
        if (!SpownMode)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject obj2 = (GameObject)Resources.Load("maskBox");
                Vector3 Setpos2 = maincamera.transform.position;
                Setpos2.z = 1f;
                Setpos2.y += 1f;
                obj2 = Instantiate(obj2, Setpos2, Quaternion.identity);
                obj2.transform.SetParent(this.transform);

                GameObject obj = (GameObject)Resources.Load("flame");
                Vector3 Setpos = maincamera.transform.position;
                Setpos.z = 1f;
                obj = Instantiate(obj, Setpos, Quaternion.identity);
                obj.transform.SetParent(this.transform);
                obj.tag = "flame";
                
                SpownMode = true;

                StopMono.enabled = true;
                WalkCon.StopWalk();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                
                SpownMode = false;
                if (OnceMove)
                {
                    WalkCon.LetsStart();
                    OnceMove = false;
                }
                else
                {
                    WalkCon.StartWalk();
                }
               
                foreach (Transform child in this.transform)
                {   
                    if (child.tag == "Mask")
                    {
                        var col = child.GetComponent<PolygonCollider2D>();
                        col.isTrigger = false;
                        child.tag = "Delete";
                    }
                    else if(child.tag=="flame")
                    {
                        var sc = child.GetComponent<FlameMove>();
                        if (sc.Mabiki())
                        {
                            sc.CreateIce();
                        }
                        else
                        {
                            Debug.Log("カットしちゃだめ");
                        }
                    }
                   
                }

                foreach (Transform child in this.transform)
                {
                    if (child.tag == "Delete")
                    {
                        Destroy(child.gameObject);
                    }
                }
                StopMono.enabled = false;
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