using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public class PlayerCon : MonoBehaviour/*,IUpdatable*/
{
    float jumppower;
    public float playerspeed;
    public float distance;
    private Rigidbody2D rb;
    Animator peguin;
    private float dir = 1f;
    //飛べる高さ
    public float height;
    //ブロック上の幅（ペンギンが乗るスペース）1の場合ブロック一個分の大きさとして扱う
    public float space;
    //飛べないと判断されたブロックキャッシュ用
    GameObject Not_JpBlock;
    float sumHeigh = 0f;

    bool jp;
    bool walk = false;
    bool StartMove;
    private float time = 0f;

    Vector2 KeepVel;
    float KeepAngl;
    Vector3 KeepPos;
    bool HitBoxCol = false;

    bool StopNow = true;
    bool JpNow;
    bool KeepJp;

    // Start is called before the first frame update
    void Start()
    {
        //this.peguin = GetComponent<Animator>();
        //rs = new Vector2(0f, jumppower);
        rb = GetComponent<Rigidbody2D>();
        jp = false;
        JpNow = false;
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
        //飛んでいる最中は判定をとらない
       
            if (col.gameObject.tag == "block" || col.gameObject.tag == "Ground")
            {
                walk = true;
                JpNow = false;
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

                Not_JpBlock = null;

                HitBoxCol = true;
                Debug.Log(dir);
            }
        }
    }


    // Update is called once per frame
    public void Update()
    {
        if (!JpNow && walk)
        {
            //PlayerRay(1);
            // peguin.SetBool("Walk", true);
        }
        if (StopNow)
        {
            transform.position = KeepPos;
        }
    }

    public void PlayerRay(int layerMask)
    {
        Vector3 _trans = transform.position;
        //足元からレイを飛ばすため
        _trans.y -= 0.2f;
        Ray2D ray = new Ray2D(_trans, new Vector2(dir, 0));

        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 0.1f, false);

        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, distance, layerMask);


        if (hit.collider)
        {


            //当たったオブジェクトの子供を取得（子がLocalScaleを保持しているため）
            var HitChild = hit.collider.gameObject.transform.GetChild(0);
            var HitChildScale = HitChild.transform.localScale;

            float Check_heght = HitChildScale.y;
            //一個目のブロックが飛べる高さの場合
            if (height > Check_heght)
            {
                //-------------------------------上にブロックが積んである場合のチェック------------------------
                //rayにヒットしたオブジェクトの左or右側からスペースが開いてるか判断
                Vector3 JumpSpace = hit.collider.transform.position;
                JumpSpace.x += (dir * -1) * (HitChildScale.x * 0.5f);
                JumpSpace.x += dir * space;
                //上に飛ばすようのray作成
                Ray2D rayUp = new Ray2D(JumpSpace, new Vector2(0, 1));
                //上に重なっていないかのチェック
                RaycastHit2D[] Uphit = Physics2D.RaycastAll(rayUp.origin, rayUp.direction, 100, layerMask);

                //Debug.DrawRay(JumpSpace, rayUp.direction * distance, Color.green, 5f, false);
                sumHeigh = 0f;

                foreach (var col in Uphit)
                {
                    sumHeigh += col.collider.transform.GetChild(0).transform.localScale.y;
                }

                if (height > sumHeigh)
                {
                    jp = true;
                    walk = false;
                    //飛んだ場合なので削除
                    //当たったオブジェクトをKEEPしておく
                    Not_JpBlock = hit.collider.gameObject;
                }
                //----------------------------------------------------------------------------------------------
            }
        }

    }

    public void StopWalk()
    {
        if (StartMove)
        {
            walk = false;
            KeepVel = rb.velocity;
            KeepAngl = rb.angularVelocity;

            rb.Sleep();

            KeepJp = jp;
            jp = false;
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
            rb.velocity = KeepVel;
            rb.angularVelocity = KeepAngl;

            if (StopNow)
            {
                jp = KeepJp;
            }
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
