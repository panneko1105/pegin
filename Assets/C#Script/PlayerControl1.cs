using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;
//1から作った方がいい気がしてきた05/18
//(1から作ることになったらこのScript使う)
public class PlayerControl1 : MonoBehaviour/*,IUpdatable*/
{
    public float jumppower;
    public float playerspeed;
    public float distance;
    private Rigidbody2D rb;

    private int dir = 1;
    //飛べる高さ
    public float height;
    //ブロック上の幅（ペンギンが乗るスペース）1の場合ブロック一個分の大きさとして扱う
    public float space;

    bool jp;

    private float time = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //rs = new Vector2(0f, jumppower);
        rb = GetComponent<Rigidbody2D>();
        jp = false;
    }

    void FixedUpdate()
    {
       
        rb.velocity = new Vector2(transform.localScale.x *dir* playerspeed*Time.deltaTime, rb.velocity.y);

        
        if (jp)
        {
            Debug.Log("jsa");
            //rb.AddForce(Vector2.up * jumppower,ForceMode2D.Impulse);
            Vector2 jump = new Vector2(0, jumppower);
            this.rb.AddForce(jump, ForceMode2D.Impulse);
            jp = false;
            
        }
        
    }

    // Update is called once per frame
    public void Update()
    {
        if (!jp)
        {
            PlayerRay(1);
        }
    }

    public void PlayerRay(int layerMask)
    {
        Vector3 _trans = transform.position;
        _trans.y -= 0.5f;
        Ray2D ray = new Ray2D(_trans, new Vector2(dir, 0));


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
                JumpSpace.x += (dir*-1) * (HitChildScale.x * 0.5f);
                JumpSpace.x += dir * space;
                //上に飛ばすようのray作成
                Ray2D rayUp = new Ray2D(JumpSpace, new Vector2(0, 1));
                //上に重なっていないかのチェック
                RaycastHit2D[] Uphit = Physics2D.RaycastAll(rayUp.origin, rayUp.direction, 100, layerMask);

                Debug.DrawRay(JumpSpace, rayUp.direction * distance, Color.green, 5f, false);
                float sumHeigh = 0f;
                
                foreach (var col in Uphit)
                {
                    sumHeigh += col.collider.transform.GetChild(0).transform.localScale.y;
                   
                }

                if (height >= sumHeigh)
                {
                    jp = true;
                }
                //----------------------------------------------------------------------------------------------

            }
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "block")
        {
            dir *= -1;

            //z軸を軸にして45度回転させるQuaternionを作成
            Quaternion rot = Quaternion.Euler(0, 180, 0);
            // 現在の自身の回転の情報を取得する
            Quaternion q = this.transform.rotation;
            this.transform.rotation = q * rot;
        }
    }
}
