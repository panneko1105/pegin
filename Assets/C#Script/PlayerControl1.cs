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
    int HitNum = 0;
    bool Jp;
    //反転判定
    bool HantenFg;
    //jpフラグをonにするか
    bool HitJpCheck;
    bool HitWall;
    GameObject penguinChild;

    //------------------実験-----------------------------------------------------------
    [System.NonSerialized]
    public Vector3 groundNormal = Vector3.zero;

    private Vector3 lastGroundNormal = Vector3.zero;

    [System.NonSerialized]
    public Vector3 lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);

    protected float groundAngle = 0;
    //--------------------------------------------------------------------------------


    // Start is called before the first frame update
    void Start()
    {
        penguinChild = transform.GetChild(0).gameObject;
        peguin = penguinChild.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
      
        StartMove = false;
        KeepPos = transform.position;
        Jp = false;
        HantenFg = false;
        HitJpCheck = false;
        HitWall = false;
        dir = 1;
    }

    void FixedUpdate()
    {
        if (walk)
        {
            rb.velocity = new Vector2(transform.localScale.x * Time.deltaTime * playerspeed, rb.velocity.y);
                    }
        if (Jp)
        {
            rb.velocity = new Vector2(transform.localScale.x * Time.deltaTime * playerspeed, 5.3f);        
            Jp = false;
        }
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (!HantenFg)
        {
            if (col.gameObject.tag == "block")
            {
                HitJpCheck = true;
            }

        }
        if (col.gameObject.tag == "Wall")
        {
            HitWall = true;
        }
    }


    // Update is called once per frame
    public void Update()
    {
        if (StopNow)
        {
            transform.position = KeepPos;
        }
        else
        {
            if (HitJpCheck)
            {
                Jp = true;
                HitJpCheck = false;
            }
            if (HitWall||HantenFg)
            {
                //反転処理
                Vector3 temp = gameObject.transform.localScale;

                //localScale.xに-1をかける

                temp.x *= -1f;

                //結果を戻す

                gameObject.transform.localScale = temp;
                dir *= -1;

                HitBoxCol = true;
                Jp = false;
                HitWall = false;
                HantenFg = false;
            }
            
        }
    }

   
    public void StopWalk()
    {
        if (StartMove)
        {
            walk = false;
      
            KeepPos = transform.position;

            StopNow = true;
            rb.Sleep();
        }
    }

    public void StartWalk()
    {
        if (StartMove)
        {
            walk = true;

            rb.WakeUp();
           
            StopNow = false;
            rb.WakeUp();
        }
    }

    public void LetsStart()
    {
        StartMove = true;
        walk = true;
        StopNow = false;
    }

    public void HitChild()
    {
        HantenFg = true;
    }

}
