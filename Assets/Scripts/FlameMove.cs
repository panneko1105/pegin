using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class FlameMove : MonoBehaviour
{
    public float move = 0.125f;
    [HideInInspector] public int flg = 0;
    [HideInInspector] public bool isRot = false;
    private List<Vector3> v = new List<Vector3>();

    PolygonCollider2D m_ObjectCollider;

    private GameObject _child;

    // Start is called before the first frame update
    void Start()
    {
        m_ObjectCollider = GetComponent<PolygonCollider2D>();
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
        if (Input.GetKeyDown(KeyCode.Return))
        {
            m_ObjectCollider.isTrigger = false;
            this.gameObject.AddComponent<Rigidbody2D>();
            var script = GetComponent<FlameMove>();

            var targetCollider = this.gameObject.GetComponent<Collider2D>();

            var overlappingColliders = new List<Collider2D>();
            targetCollider.OverlapCollider(new ContactFilter2D(), overlappingColliders);
            var carvers = overlappingColliders.Select(c => c.GetComponentInChildren<Carver>())
                .Where(c => c != null);
            var thisCarver = targetCollider.GetComponentInChildren<Carver>();

            Debug.Log(
                $"Carve {targetCollider.name} with {string.Join(", ", carvers.Select(c => c.Collider2D.name))}.");
            Carver.Carve(thisCarver, carvers);

            DrawMesh dr = this.gameObject.GetComponent<DrawMesh>();
            MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();
            Vector3[] test = mf.mesh.vertices;

           
            foreach (Vector3 item in test)
            {
                v.Add(item);
                Debug.Log(item);
            }
            dr.CreateMesh(v);

            var ChangeMaterial = transform.GetChild(0).gameObject.GetComponent<Carver>();

            this.GetComponent<Renderer>().sharedMaterial = ChangeMaterial.GetMaterial();


            ChangeMaterial.Change();
            Destroy(script);
           
        }
    }
 
}
