using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.VFX;
using GokUtil.UpdateManager;


public class FlameMove : MonoBehaviour, IUpdatable { 
    public float move = 5.0f;
    [HideInInspector] public int flg = 0;
    [HideInInspector] public bool isRot = false;        //trueで傾いている
    private List<Vector3> v = new List<Vector3>();
    private List<GameObject> TrigeerStayObj=new List<GameObject>();
    List<Vector3> myPoint = new List<Vector3>();
    List<Vector3> Dainyuu = new List<Vector3>();

    private List<GameObject> GR_Child = new List<GameObject>();

    static int num = 0;

    PolygonCollider2D m_ObjectCollider;

    private GameObject _child;


    private DrawMesh dr;
    // Start is called before the first frame update
    void Start()
    {
        m_ObjectCollider = GetComponent<PolygonCollider2D>();

        dr = this.gameObject.GetComponent<DrawMesh>();
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
        if (PauseManager.Instance.GetisPause())
        {
            return;
        }
        // 上移動
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.position += new Vector3(0, 1f, 0);
        }
        // 下移動
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.position += new Vector3(0, -1f, 0);
        }
        // 左に移動
        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.position += new Vector3(1f, 0, 0);
        }
        // 右に移動
        if (Input.GetKeyDown(KeyCode.A))
        {
            transform.position += new Vector3(-1f, 0, 0);
        }
        // 45度回転
        // 左回転
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Quaternion myRot = this.transform.rotation;
            Quaternion rot = Quaternion.Euler(0, 0, 1);

            // 現在の自身の回転の情報を取得する
            this.transform.rotation = myRot * rot;

         

            isRot = !isRot;
        }
        // 右回転
        if (Input.GetKey(KeyCode.RightArrow)) {
            Quaternion myRot = this.transform.rotation;
            Quaternion rot = Quaternion.Euler(0, 0, -1);

            // 現在の自身の回転の情報を取得する
            this.transform.rotation = myRot * rot;


            isRot = !isRot;
        }


        if (Input.GetKeyDown(KeyCode.J))
        {
            var mago = transform.GetChild(0);
            for (int i = 0; i < 4; i++)
            {
                GR_Child.Add(mago.GetChild(i).gameObject);
                
            }
            mago.transform.DetachChildren();

            foreach(GameObject K_pos in GR_Child)
            {
                myPoint.Add(K_pos.transform.position);
            }
            Mabiki();
        }
    }

    public void CreateIce()
    {

        //エフェクト発生
        GameObject obj = (GameObject)Resources.Load("test");
        Instantiate(obj, transform.position, Quaternion.identity);


        m_ObjectCollider.isTrigger = false;
        var rb = this.gameObject.AddComponent<Rigidbody2D>();
        rb.mass = 100f;

        this.gameObject.layer = 0;

        var targetCollider = this.gameObject.GetComponent<Collider2D>();
        this.gameObject.tag = "block";
        transform.GetChild(0).gameObject.tag = "block";

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
        MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();
        Vector3[] test = mf.mesh.vertices;


        foreach (Vector3 item in test)
        {
            v.Add(item);
        }
        dr.CreateMesh(v);
        var child = transform.GetChild(0);

        //子のメッシュを削除
        var delfilter = child.GetComponent<MeshFilter>();
        var delrender = child.GetComponent<MeshRenderer>();

        Destroy(delfilter);
        Destroy(delrender);

        //子のスクリプトを取得してセットしてあるマテリアルを取得
        var ChangeMaterial = child.gameObject.GetComponent<Carver>();
        this.GetComponent<Renderer>().sharedMaterial = ChangeMaterial.GetMaterial();

        //managerからDelete関数を呼び出す
        var Deletescript = transform.root.gameObject.GetComponent<CreateFlame>();
        Deletescript.DeleteChild();

        ChangeMaterial.Change();
        //名前付与
        this.gameObject.name = "ice" + num;
        num++;
        Destroy(this);

    }
 
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag =="block")
        {
            //現在接触中のオブジェクトのリストに追加
            TrigeerStayObj.Add(col.gameObject);  
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "block")
        {
            foreach(GameObject item in TrigeerStayObj)
            {
                //離れたオブジェクトの削除
                if (col.name == item.name)
                {
                    //リストから削除
                    TrigeerStayObj.Remove(item);
                   
                }
            }
        }
    }

    //点と多角形の内外判定
    static public bool Check(Vector3[] points, Vector3 target, Vector3 normal)
    {
        // XY平面上に写像した状態で計算を行う
        Quaternion rot = Quaternion.FromToRotation(normal, -Vector3.forward);

        Vector3[] rotPoints = new Vector3[points.Length];

        for (int i = 0; i < rotPoints.Length; i++)
        {
            rotPoints[i] = rot * points[i];
        }

        target = rot * target;

        int wn = 0;
        float vt = 0;

        for (int i = 0; i < rotPoints.Length; i++)
        {
            // 上向きの辺、下向きの辺によって処理を分ける

            int cur = i;
            int next = (i + 1) % rotPoints.Length;

            // 上向きの辺。点PがY軸方向について、始点と終点の間にある。（ただし、終点は含まない）
            if ((rotPoints[cur].y <= target.y) && (rotPoints[next].y > target.y))
            {
                // 辺は点Pよりも右側にある。ただし重ならない
                // 辺が点Pと同じ高さになる位置を特定し、その時のXの値と点PのXの値を比較する
                vt = (target.y - rotPoints[cur].y) / (rotPoints[next].y - rotPoints[cur].y);

                if (target.x < (rotPoints[cur].x + (vt * (rotPoints[next].x - rotPoints[cur].x))))
                {
                    // 上向きの辺と交差した場合は+1
                    wn++;
                }
            }
            else if ((rotPoints[cur].y > target.y) && (rotPoints[next].y <= target.y))
            {
                // 辺は点Pよりも右側にある。ただし重ならない
                // 辺が点Pと同じ高さになる位置を特定し、その時のXの値と点PのXの値を比較する
                vt = (target.y - rotPoints[cur].y) / (rotPoints[next].y - rotPoints[cur].y);

                if (target.x < (rotPoints[cur].x + (vt * (rotPoints[next].x - rotPoints[cur].x))))
                {
                    // 下向きの辺と交差した場合は-1
                    wn--;
                }
            }
        }

        return wn != 0;
    }

    //線分の交差判定
    public static bool LineSegmentsIntersection(Vector2 p1,Vector2 p2,Vector2 p3,Vector3 p4,out Vector2 intersection)
    {
        intersection = Vector2.zero;

        var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0.0f)
        {
            return false;
        }

        var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        intersection.x = p1.x + u * (p2.x - p1.x);
        intersection.y = p1.y + u * (p2.y - p1.y);

        return true;
    }

    void Mabiki()
    {
        Vector3 ireru = new Vector3(0, 0, 0);
        Vector3[] watasu = {ireru, ireru , ireru, ireru };
        foreach (Transform child in transform.root.transform)
        {
            if (child.tag == "Mask")
            {
                
                float mx_pos = child.transform.position.x;
                float my_pos = child.transform.position.y;
               
                float mx_ = (mx_pos + 1.5f);
                float my_ = my_pos + 1.5f;
                watasu[0] = new Vector3(mx_, my_, 1f);

                mx_ = (mx_pos + 1.5f);
                my_ = my_pos - 1.5f;
                watasu[1] = new Vector3(mx_, my_, 1f);
                
                mx_ = mx_pos - 1.5f;
                my_ = my_pos - 1.5f;
                watasu[2] = new Vector3(mx_, my_, 1f);
                
                mx_ = (mx_pos -1.5f);
                my_ = my_pos + 1.5f;
                watasu[3] = new Vector3(mx_, my_, 1f);
                
            }
        }
        
        //マスク内に入ってる
        int InNum = 0;
        //何番目の頂点がはいってるか
        int[] VtxNum = { 4, 5, 6 };
        //点が何個はいってるか
        int VtxCnt = 0;
        for (int i = 0; i < 4; i++)
        {
            if (Check(watasu, myPoint[i], new Vector3(0, 0, -1f)))
            {
                InNum++;
                VtxNum[VtxCnt] = i;
                VtxCnt++;
            }
        }
        int kazu = 0;
        foreach(Vector3 item in myPoint)
        {
            if (kazu != VtxNum[0] && kazu != VtxNum[1] && kazu != VtxNum[2])
            {
                Dainyuu.Add(item);
            }
            kazu++;
        }
       
        Vector2 Kousa;
        Vector3[] kouten = { ireru, ireru };
        int CrossCnt = 0;
        for(int i = 0; i < 3; i++)
        {
            if(LineSegmentsIntersection(watasu[1],watasu[2],myPoint[i],myPoint[i+1],out Kousa))
            {
                if(CheckUpVec(myPoint[i], myPoint[i + 1]))
                {
                    kouten[1] = new Vector3(Kousa.x, Kousa.y, 1f);
                }
                else
                {
                    kouten[0] = new Vector3(Kousa.x, Kousa.y, 1f);
                }
                
                CrossCnt++;
            }
            if (i == 2)
            {
                if(LineSegmentsIntersection(watasu[1], watasu[2], myPoint[3], myPoint[0], out Kousa))
                {
                    if (CheckUpVec(myPoint[3], myPoint[0]))
                    {
                        kouten[1] = new Vector3(Kousa.x, Kousa.y, 1f);
                    }
                    else
                    {
                        kouten[0] = new Vector3(Kousa.x, Kousa.y, 1f);
                    }
                    CrossCnt++;
                }
            }
        }

        Vector3[] NewPoint = new Vector3[6 - InNum];
        switch (InNum)
        {
            case 0:
                Debug.Log("4角形");
                break;
            case 1:
                Debug.Log("5角形");
                NewPoint[0] = kouten[1];
                NewPoint[1] = kouten[0];

                NewPoint[2] = Dainyuu[0];
                NewPoint[3] = Dainyuu[1];
                NewPoint[4] = Dainyuu[2];

                break;
            case 2:
                Debug.Log("4角形");
                NewPoint[0] = kouten[1];
                NewPoint[1] = kouten[0];

                NewPoint[2] = Dainyuu[0];
                NewPoint[3] = Dainyuu[1];               
                break;
            case 3:
                Debug.Log("３角形");
                NewPoint[0] = kouten[1];
                NewPoint[1] = kouten[0];
                NewPoint[2] = Dainyuu[0];
                break;
            case 4:
                Debug.Log("0角形");
                break;
        }


    }

    //上向きならtrue
    bool CheckUpVec(Vector2 p1, Vector2 p2)
    {
        bool Upfg = false;
        if (p1.y > p2.y)
        {
            Upfg = false;
        }
        else
        {
            Upfg = true;
        }
        return Upfg;
    }
}
