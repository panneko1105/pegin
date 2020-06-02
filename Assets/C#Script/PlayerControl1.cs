using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public class PlayerControl1 : MonoBehaviour/*,IUpdatable*/
{

    public float playerspeed;
    private Rigidbody2D rb;
    Animator peguin;
    private float dir = 1f;

    bool walk = false;
    bool StartMove;

    Vector3 KeepPos;
    bool HitBoxCol = false;

    bool StopNow = true;
 
    // Start is called before the first frame update
    void Start()
    {
        this.peguin = GetComponent<Animator>();
        //rs = new Vector2(0f, jumppower);
        rb = GetComponent<Rigidbody2D>();
      
        StartMove = false;
        KeepPos = transform.position;
        //jp = anime.SetBool("Jump",false);
    }

    void FixedUpdate()
    {
        if (walk)
        {
            rb.velocity = new Vector2(transform.localScale.x * Time.deltaTime * playerspeed, rb.velocity.y);
        }
    }


    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "block" || col.gameObject.tag == "Ground")
        {
            walk = true;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!StopNow)
        {
            //歩き出すよう
            if (col.gameObject.tag == "block")
            {
                //反転処理
                Vector3 temp = gameObject.transform.localScale;

                //localScale.xに-1をかける

                temp.x *= -1f;

                //結果を戻す

                gameObject.transform.localScale = temp;
                dir *= -1;

                HitBoxCol = true;
                Debug.Log(dir);
            }
        }
    }


    // Update is called once per frame
    public void Update()
    {
        if (StopNow)
        {
            transform.position = KeepPos;
        }
    }

   
    public void StopWalk()
    {
        if (StartMove)
        {
            walk = false;
      
            KeepPos = transform.position;

            StopNow = true;
        }
    }

    public void StartWalk()
    {
        if (StartMove)
        {
            walk = true;

            rb.WakeUp();
           
            StopNow = false;
        }
    }

    public void LetsStart()
    {
        StartMove = true;
        walk = true;
        StopNow = false;
    }

}
