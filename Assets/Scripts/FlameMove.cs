using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.VFX;
using GokUtil.UpdateManager;


public class FlameMove : MonoBehaviour, IUpdatable { 
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

    void OnEnable()
    {
        UpdateManager.AddUpdatable(this);
    }

    void OnDisable()
    {
        UpdateManager.RemoveUpdatable(this);
    }


    // Update is called once per frame
    public void UpdateMe()
    {
        // 関係ないときは動かせない
        if (StageManager.Instance.GetFlg() != StageFlg.NOMAL)
        {
            return;
        }

        // 上移動
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, move, 0);
        }
        // 下移動
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0, -move, 0);
        }
        // 左に移動
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(move, 0, 0);
        }
        // 右に移動
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-move, 0, 0);
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

        
        // Enterキーが押されたときの処理をここに書く
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // CubeプレハブをGameObject型で取得
            GameObject obj = (GameObject)Resources.Load("test");

            Instantiate(obj, transform.position, Quaternion.identity);


            m_ObjectCollider.isTrigger = false;
            this.gameObject.AddComponent<Rigidbody2D>();
            
            this.gameObject.layer = 0;

            var targetCollider = this.gameObject.GetComponent<Collider2D>();
            this.gameObject.tag = "block";

            //切り取り
            var overlappingColliders = new List<Collider2D>();
            targetCollider.OverlapCollider(new ContactFilter2D(), overlappingColliders);
            var carvers = overlappingColliders.Select(c => c.GetComponentInChildren<Carver>())
                .Where(c => c != null);
            var thisCarver = targetCollider.GetComponentInChildren<Carver>();

            Debug.Log(
                $"Carve {targetCollider.name} with {string.Join(", ", carvers.Select(c => c.Collider2D.name))}.");
            Carver.Carve(thisCarver, carvers);

            //メッシュの生成
            DrawMesh dr = this.gameObject.GetComponent<DrawMesh>();
            MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();
            Vector3[] test = mf.mesh.vertices;

           
            foreach (Vector3 item in test)
            {
                v.Add(item);
            }
            dr.CreateMesh(v);

            var ChangeMaterial = transform.GetChild(0).gameObject.GetComponent<Carver>();

            this.GetComponent<Renderer>().sharedMaterial = ChangeMaterial.GetMaterial();


            ChangeMaterial.Change();
            Destroy(this);
           
        }
    }
 
}
