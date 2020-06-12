using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakWall : MonoBehaviour
{
    bool BreakFg = false;
    GameObject KeepBlock;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "block")
        {
            if (KeepBlock == collision.gameObject)
            {
                var rb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (Mathf.Abs(collision.relativeVelocity.x) > 1f)
                {
                    //壁破壊！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
                    // SE
                    SoundManager.Instance.PlaySeEX("溶ける音CESA");

                    //エフェクト発生
                    GameObject obj = (GameObject)Resources.Load("icebreak");
                    Vector3 EfectPos = transform.position;
                    //足元から出すため少し下げる
                    EfectPos.y -= 1.0f;
                    Instantiate(obj, EfectPos, Quaternion.identity);

                    Destroy(this.gameObject);
                    //ぶつかってきた氷の速度を原作させて与えなおす
                    rb.velocity = new Vector2(collision.relativeVelocity.x * 0.3f, collision.relativeVelocity.x * 0.3f);
                }
            }
        }

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "block")
        {
            //横から当たってきた氷がそのまま本体に当たったか確認するため
            KeepBlock = col.gameObject;
        }
    }
}
