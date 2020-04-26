using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turn : MonoBehaviour
{
    float rayx = 1f;
    public float distance = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int LayerObject = LayerMask.GetMask(new string[] { "Default" });
        Vector2 pos = new Vector2(transform.position.x, 0);
        Vector2 dir = new Vector2(rayx, 0);
        RaycastHit2D hit = Physics2D.Raycast(pos, dir, distance, LayerObject);


        float duration = 0.0f;   // 表示期間（秒）

        Debug.DrawRay(pos, dir * distance, Color.red, duration, false);

        if (hit.collider)
        {
            Debug.Log(hit.collider.gameObject.name);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "block")
        {
            Quaternion rot = Quaternion.Euler(0, 180, 0);
            // 現在の自身の回転の情報を取得する
            Quaternion q = this.transform.rotation;
            this.transform.rotation = q * rot;

            rayx *= -1;
            Debug.Log(rayx);
        }

    }
}
