using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetStar : MonoBehaviour
{
    public GameObject Manager;
    public int MyNumber;

    ItemManager sc;
    void Start()
    {
        sc = Manager.GetComponent<ItemManager>();
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 30.0f * Time.deltaTime, 0));
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag =="Player")
        {
            sc.ItemGetting(MyNumber);
            Destroy(this.gameObject);
        }
    }
}
