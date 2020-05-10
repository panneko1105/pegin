using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public class PlayerMove : MonoBehaviour, IUpdatable
{
    public float jumppower = 10;
    public float playerspeed = 0.1f;
    private Rigidbody2D rb;
    public float distance;
    //飛べる高さ
    public float height;
    //ブロック上の幅（ペンギンが乗るスペース）
    public float space;

    private int dir = 1;

    private bool walk;

    private bool jp;

    private float time = 0f;
    void OnEnable()
    {
        UpdateManager.AddUpdatable(this);
    }

    void OnDisable()
    {
        UpdateManager.RemoveUpdatable(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        walk = true;
        jp = false;
    }

    void FixedUpdate()
    {
        if (walk)
        {
            rb.velocity = new Vector2(transform.localScale.x * playerspeed, rb.velocity.y);
        }
        else
        {
            if (jp)
            {
                time += Time.deltaTime;
                rb.velocity = new Vector2(0f, 0f);
                if (time > 0.4f)
                {
                    rb.velocity = new Vector2(dir*1.5f, 7f);
                    jp = false;
                }
            }
        }
    }

    // Update is called once per frame
    public void UpdateMe()
    {
        PlRay(1);
    }

    //向き反転用
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "block")
        {
            Vector3 temp = gameObject.transform.localScale;

            //localScale.xに-1をかける

            temp.x *= -1;

            //結果を戻す

            gameObject.transform.localScale = temp;

            dir *= -1;

        }
    }
    //歩き出すよう
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "block")
        {
            walk = true;
            time = 0f;
        }
    }

    //２D用
    public void PlRay(int layerMask)
    {
        Ray2D ray = new Ray2D(transform.position, new Vector2(dir, 0));

        layerMask = 1;

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, distance, layerMask);
        //飛びます
        if (hit.collider)
        {
            var child = hit.collider.gameObject.transform.GetChild(0);
            var size = child.transform.localScale;
            //設定された高さより低かった場合
            if (transform.position.y + height > hit.collider.gameObject.transform.position.y + size.y/2.0f)
            {
                //当たったブロックの左側の座標               
                float hitleft = hit.collider.gameObject.transform.position.x - size.x / 2.0f;

                Vector3 raypos;
                raypos.x = hitleft + space;
                raypos.y = hit.collider.gameObject.transform.position.y + size.y/2.0f + 0.1f;
                raypos.z = 1.0f;

                ray = new Ray2D(raypos, new Vector2(0, 1));
                Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 0.1f, false);
                RaycastHit2D hit2 = Physics2D.Raycast(ray.origin, ray.direction, distance, layerMask);

                if (hit2.collider)
                {
                    walk = true;
                }
                else
                {
                    walk = false;
                    jp = true;
                }
            }
        }

       
    }
}
