using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public class PlayerMove : MonoBehaviour, IUpdatable
{
    public float jumppower = 10;
    float playerspeed = 0.1f;
    private Rigidbody2D rb;
    public float distance;
    public float height;

    private int dir = 1;
    private bool rayhit;

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
        rayhit = false;
    }

    void FixedUpdate()
    {

        //localScale.xには1か-1が入る

        rb.velocity = new Vector2(transform.localScale.x * playerspeed, rb.velocity.y);

    }

    // Update is called once per frame
    public void UpdateMe()
    {

        PlRay(1);
    }

    //2Dの衝突判定
    void OnCollisionEnter2D(Collision2D col)
    {
        if (rayhit)
        {
            if (col.gameObject.tag == "block")
            {

                Vector3 temp = gameObject.transform.localScale;

                //localScale.xに-1をかける

                temp.x *= -1;

                //結果を戻す

                gameObject.transform.localScale = temp;

                dir *= -1;
                rayhit = false;

            }
        }

    }

    //２D用
    public void PlRay(int layerMask)
    {
        bool jumpflg = false;
        Ray2D ray = new Ray2D(transform.position, new Vector2(dir, 0));

        layerMask = 1;

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, distance, layerMask);
        Debug.DrawRay(ray.origin, ray.direction, Color.green);
        //飛びます
        if (hit.collider)
        {
         
            //一番上の頂点を入れる
            float maxheight = -1f; ;

            Mesh myMesh = hit.collider.GetComponent<MeshFilter>().mesh;
            for (int i = 0; i < myMesh.vertices.Length; i++)
            {
                if (maxheight < myMesh.vertices[i].y)
                {
                    maxheight = myMesh.vertices[i].y;
                }
            }
            maxheight *= hit.collider.transform.lossyScale.y;

            //設定された高さより低かった場合　　飛んだ場合
            if (transform.position.y + height > hit.collider.transform.position.y + maxheight)
            {
                jumpflg = true;
                rb.AddForce(Vector2.up * jumppower);
                rayhit = false;
            }
            else
            {
                rayhit = true;
            }
        }

       
    }
}
