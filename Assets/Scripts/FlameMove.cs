using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameMove : MonoBehaviour
{
    public float move = 0.05f;
    [HideInInspector] public int flg = 0;
    [HideInInspector] public bool isRot = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 上移動
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, move, 0 * Time.deltaTime);
        }
        // 下移動
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0, -move, 0 * Time.deltaTime);
        }
        // 左に移動
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(move, 0, 0 * Time.deltaTime);
        }
        // 右に移動
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-move, 0, 0 * Time.deltaTime);
        }
        // 45度回転
        // 左回転
        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            //z軸を軸にして45度回転させるQuaternionを作成
            Quaternion rot = Quaternion.Euler(0, 0, 45);
            // 現在の自身の回転の情報を取得する
            Quaternion q = this.transform.rotation;
            this.transform.rotation = q * rot;
            isRot = !isRot;
        }
        // 右回転
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            //z軸を軸にして-45度回転させるQuaternionを作成
            Quaternion rot = Quaternion.Euler(0, 0, -45);
            //現在の自身の回転の情報を取得する
            Quaternion q = this.transform.rotation;
            this.transform.rotation = q * rot;
            isRot = !isRot;
        }

        // 水増し
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            flg++;
            if (flg >= 3)
            {
                flg = 3;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            flg--;
            if (flg < 0)
            {
                flg = 0;
            }
        }

        // Enterキーが押されたときの処理をここに書く
        if (Input.GetKeyDown(KeyCode.Return)) {

        }
    }
    void FixedUpdate()
    {

    }
}
