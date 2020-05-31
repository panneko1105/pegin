using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public class PlayerControl1 : MonoBehaviour/*,IUpdatable*/
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

        if (jp)
        {
            time += Time.deltaTime;
            Vector2 tame = rb.velocity;
            tame.x = 0f;
            rb.velocity = tame;
            if (time > 0.8f)
            {
                // peguin.SetBool("Walk", false);
                //rb.AddForce(Vector2.up * jumppower,ForceMode2D.Impulse);
                //this.rb.AddForce(jump, ForceMode2D.Impulse);
                // peguin.SetTrigger("Jump");
                jp = false;
                JpNow = true;

                Vector3 Jp_pos = Not_JpBlock.transform.position;
                Jp_pos.y += Not_JpBlock.transform.localScale.y*0.5f;
                //Shoot(Jp_pos);
                jumppower = 5.6f + 1.3f * sumHeigh;
                if (dir > 0f)
                {
                    rb.velocity = new Vector2(1.5f, jumppower);
                }
                else
                {
                    rb.velocity = new Vector2(-1.5f, jumppower);
                }
                time = 0f;
            }
        }
    }

    
    void OnCollisionEnter2D(Collision2D col)
    {
        //飛んでいる最中は判定をとらない
        if (JpNow)
        {
            if (col.gameObject.tag == "block" || col.gameObject.tag == "Ground")
            {
                walk = true;
                JpNow = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!JpNow && !StopNow)
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
        if (!JpNow&&walk)
        {
            PlayerRay(1);
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



    private void Shoot(Vector3 i_targetPosition)
    {
        // とりあえず適当に60度でかっ飛ばすとするよ！
        ShootFixedAngle(i_targetPosition, 60.0f);
    }

    private void ShootFixedAngle(Vector3 i_targetPosition, float i_angle)
    {
        float speedVec = ComputeVectorFromAngle(i_targetPosition, i_angle);
        if (speedVec <= 0.0f)
        {
            // その位置に着地させることは不可能のようだ！
            Debug.LogWarning("!!");
            return;
        }

        Vector3 vec = ConvertVectorToVector3(speedVec, i_angle, i_targetPosition);

        Vector3 force = vec * rb.mass;

        rb.AddForce(force, ForceMode2D.Impulse);
    }

    private float ComputeVectorFromAngle(Vector3 i_targetPosition, float i_angle)
    {
        // xz平面の距離を計算。
        Vector2 startPos = new Vector2(transform.position.x,transform.position.z);
        Vector2 targetPos = new Vector2(i_targetPosition.x, i_targetPosition.z);
        float distance = Vector2.Distance(targetPos, startPos);

        float x = distance;
        float g = Physics.gravity.y;
        float y0 = transform.position.y - 0.2f;
        float y = i_targetPosition.y;

        // Mathf.Cos()、Mathf.Tan()に渡す値の単位はラジアンだ。角度のまま渡してはいけないぞ！
        float rad = i_angle * Mathf.Deg2Rad;

        float cos = Mathf.Cos(rad);
        float tan = Mathf.Tan(rad);

        float v0Square = g * x * x / (2 * cos * cos * (y - y0 - x * tan));

        // 負数を平方根計算すると虚数になってしまう。
        // 虚数はfloatでは表現できない。
        // こういう場合はこれ以上の計算は打ち切ろう。
        if (v0Square <= 0.0f)
        {
            return 0.0f;
        }

        float v0 = Mathf.Sqrt(v0Square);
        return v0;
    }

    private Vector3 ConvertVectorToVector3(float i_v0, float i_angle, Vector3 i_targetPosition)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = i_targetPosition;
        startPos.y = 0.0f;
        targetPos.y = 0.0f;

        Vector3 dir = (targetPos - startPos).normalized;
        Quaternion yawRot = Quaternion.FromToRotation(Vector3.right, dir);
        Vector3 vec = i_v0 * Vector3.right;

        vec = yawRot * Quaternion.AngleAxis(i_angle, Vector3.forward) * vec;

        return vec;
    }

}
