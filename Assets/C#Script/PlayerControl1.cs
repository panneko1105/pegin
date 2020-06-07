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

    GameObject penguinChild;
    // Start is called before the first frame update
    void Start()
    {
        penguinChild = transform.GetChild(0).gameObject;
        peguin = penguinChild.GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
      
        StartMove = false;
        KeepPos = transform.position;
        Jp = false;
    }

    void FixedUpdate()
    {
        if (walk)
        {
            rb.velocity = new Vector2(transform.localScale.x * Time.deltaTime * playerspeed, rb.velocity.y);
        }
        if (Jp)
        {
            rb.velocity = new Vector2(transform.localScale.x * Time.deltaTime * playerspeed, 3f);
            Jp = false;
        }
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (!StopNow)
        {
            //歩き出すよう
            if (col.gameObject.tag == "block")
            {
                HitNum++;
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
        else
        {
            if (HitNum == 2)
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
                HitNum = 0;
            }
            else if (HitNum == 1)
            {
                SmallJump();
                HitNum = 0;
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

            // SE停止
            SoundManager.Instance.StopSeEX("Step_EX");
        }
    }

    public void StartWalk()
    {
        if (StartMove)
        {
            walk = true;

            rb.WakeUp();
           
            StopNow = false;

            // SE再生
            SoundManager.Instance.PlaySeEX("Step_EX");
            Debug.Log("足音再生");
        }
    }

    public void LetsStart()
    {
        StartMove = true;
        walk = true;
        StopNow = false;

        // SE再生
        SoundManager.Instance.PlaySeEX("Step_EX");
        Debug.Log("足音再生");
    }

   public void SmallJump()
    {
        Jp = true;
    }
}
