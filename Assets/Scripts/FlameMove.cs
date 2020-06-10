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
    private List<Transform> TrigeerStayObj=new List<Transform>();
    List<Vector3> myPoint = new List<Vector3>();
    List<Vector3> Dainyuu = new List<Vector3>();
    //孫のリスト
    private List<GameObject> GR_Child = new List<GameObject>();
    //Mabiki関数一回だけ実行
    bool OnceFg = false;
    Transform mago;
    static int num = 0;
    PolygonCollider2D m_ObjectCollider;
    private GameObject _child;
    private DrawMesh dr;

    private float vibrateRange; //振動幅
    private float vibrateSpeed;               //振動速度

    private float initPosition;   //初期ポジション
    private float newPosition;    //新規ポジション
    private float minPosition;    //ポジションの下限
    private float maxPosition;    //ポジションの上限
    private bool directionToggle; //振動方向の切り替え用トグル(オフ：値が小さくなる方向へ オン：値が大きくなる方向へ)

    bool VibrateFg;
    private float VibrateTime;
    GameObject MaskCube;

    private PhysicsMaterial2D yuka;

    //bool pushFlg = false;         //!< 入力フラグ
    //int pushCnt = 0;              //!< 押し続け
    bool upDwnSeFlg = false;

    // Start is called before the first frame update
    void Start()
    {
        m_ObjectCollider = GetComponent<PolygonCollider2D>();
        dr = this.gameObject.GetComponent<DrawMesh>();
        mago = transform.GetChild(0);
        for (int i = 0; i < 4; i++)
        {
            GR_Child.Add(mago.GetChild(i).gameObject);
        }


        vibrateRange = 0.3f;
        vibrateSpeed = 6.0f;

        VibrateFg = false;
        VibrateTime = 0f;

        foreach (Transform child in transform.root.transform)
        {
            if (child.tag == "Mask")
            {
                MaskCube = child.gameObject;
            }
        }
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
        if (!VibrateFg)
        {
            // L Stick
            float lsv = Input.GetAxis("L_Stick_V");
            float lsh = Input.GetAxis("L_Stick_H");
            // 十字キー
            float dpv = Input.GetAxis("D_Pad_V");
            float dph = Input.GetAxis("D_Pad_H");
            //if (lsh != 0)
            //{
            //    Debug.Log("スティック" + lsh);
            //}
            //if (dph != 0)
            //{
            //    Debug.Log("ボタン" + dph);
            //}

            // 押してない判定
            //if (lsv == 0 && dpv == 0 && lsh == 0 && dph == 0)
            //{
            //    pushFlg = false;
            //    pushCnt = 0;
            //}

            Vector3 vec = new Vector3(0, 0, 0);
            //----------------------------------------------
            //  フレーム移動 (左スティック)
            //----------------------------------------------
            // 上移動 (キーボード)
            if (Input.GetKey(KeyCode.W))
            {
                vec = new Vector3(0, 10f * Time.deltaTime, 0);
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    vec = new Vector3(0, 2f * Time.deltaTime, 0);
                }
                transform.position += vec;
                MaskCube.transform.position += vec;
            }
            bool isUp = false;
            // 十字キー
            if (dpv > 0.0f)
            {
                vec = new Vector3(0, 2f * Time.deltaTime, 0);
                isUp = true;
            }
            // スティック
            if(lsv > 0.05f)
            {
                vec = new Vector3(0, 12f * lsv * Time.deltaTime, 0);
                isUp = true;
            }
            if (isUp)
            {
                transform.position += vec;
                MaskCube.transform.position += vec;
            }

            // 下移動 (キーボード)
            if (Input.GetKey(KeyCode.S))
            {
                vec = new Vector3(0, -10f * Time.deltaTime, 0);
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    vec = new Vector3(0, -2f * Time.deltaTime, 0);
                }
                transform.position += vec;
                MaskCube.transform.position += vec;
            }
            bool isDown = false;
            // 十字キー
            if (dpv < 0.0f)
            {
                vec = new Vector3(0, -2f * Time.deltaTime, 0);
                isDown = true;
            }
            // スティック
            if (lsv < -0.05f)
            {
                vec = new Vector3(0, 12f * lsv * Time.deltaTime, 0);
                isDown = true;
            }
            if (isDown)
            {
                transform.position += vec;
                MaskCube.transform.position += vec;
            }

            // 右に移動 (キーボード)
            if (Input.GetKey(KeyCode.D))
            {
                vec = new Vector3(10f * Time.deltaTime, 0, 0);
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    vec = new Vector3(2f * Time.deltaTime, 0, 0);
                }
                transform.position += vec;
                MaskCube.transform.position += vec;
            }
            bool isRight = false;
            // 十字キー
            if (dph > 0.0f)
            {
                vec = new Vector3(2f * Time.deltaTime, 0, 0);
                isRight = true;
            }
            // スティック
            if (lsh > 0.05f)
            {
                vec = new Vector3(12f * lsh * Time.deltaTime, 0, 0);
                isRight = true;
            }
            if (isRight)
            {
                //Debug.Log("おおおおい");
                transform.position += vec;
                MaskCube.transform.position += vec;
            }

            // 左に移動 (キーボード)
            if (Input.GetKey(KeyCode.A))
            {
                vec = new Vector3(-10f * Time.deltaTime, 0, 0);
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    vec = new Vector3(-2f * Time.deltaTime, 0, 0);
                }
                transform.position += vec;
                MaskCube.transform.position += vec;
            }
            bool isLeft = false;
            // 十字キー
            if (dph < 0.0f)
            {
                vec = new Vector3(-2f * Time.deltaTime, 0, 0);
                isLeft = true;
            }
            // スティック
            if (lsh < -0.05f)
            {
                vec = new Vector3(12f * lsh * Time.deltaTime, 0, 0);
                isLeft = true;
            }
            if (isLeft)
            {
                transform.position += vec;
                MaskCube.transform.position += vec;
            }

            //----------------------------------------------
            //  水位の上下 (LB, RB→スティックに変更)
            //----------------------------------------------
            //float triggerLR = Input.GetAxis("L_R_Trigger");
            float rsv = Input.GetAxis("R_Stick_V");
            //if (rsv != 0)
            //{
            //    Debug.Log(rsv);
            //}
            bool upDownFlg = false;

            if (Input.GetKey(KeyCode.UpArrow) || rsv > 0)
            {
                float Chek_pos = MaskCube.transform.position.y - transform.position.y;
                if (Chek_pos < 3f)
                {
                    // キーボード
                    if (rsv == 0)
                    {
                        MaskCube.transform.position += new Vector3(0, 2f * Time.deltaTime, 0);
                    }
                    // コントローラ
                    else
                    {
                        MaskCube.transform.position += new Vector3(0, 2.5f * rsv * Time.deltaTime, 0);
                    }
                }
                upDownFlg = true;
            }

            if (Input.GetKey(KeyCode.DownArrow) || rsv < 0)
            {
                float Chek_pos = MaskCube.transform.position.y - transform.position.y;
                if (Chek_pos > 0.5f)
                {
                    // キーボード
                    if (rsv == 0)
                    {
                        MaskCube.transform.position += new Vector3(0, -2f * Time.deltaTime, 0);
                    }
                    // コントローラ
                    else
                    {
                        MaskCube.transform.position += new Vector3(0, 2.5f * rsv * Time.deltaTime, 0);
                    }
                }
                upDownFlg = true;
            }

            // 上下開始でSE再生
            if (upDownFlg)
            {
                // SE再生
                if (!upDwnSeFlg)
                {
                    SoundManager.Instance.PlaySeEX("near_a_brook");
                    upDwnSeFlg = true;
                }
            }
            // 上下終了でSE停止
            else
            {
                // SE停止
                if (upDwnSeFlg)
                {
                    Debug.Log("SEストップ");
                    //SoundManager.Instance.StopSe();
                    SoundManager.Instance.StopSeEX("near_a_brook");
                    upDwnSeFlg = false;
                }
            }
            
            //----------------------------------------------
            //  フレーム回転 (LR)
            //----------------------------------------------
            // 左回転
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey("joystick button 4"))
            {
                Quaternion myRot = this.transform.rotation;
                Quaternion rot = Quaternion.Euler(0, 0, 60f * Time.deltaTime);
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    rot = Quaternion.Euler(0, 0, 20f * Time.deltaTime);
                }
                // 現在の自身の回転の情報を取得する
                this.transform.rotation = myRot * rot;

                isRot = !isRot;
            }
            // 右回転
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey("joystick button 5"))
            {
                Quaternion myRot = this.transform.rotation;
                Quaternion rot = Quaternion.Euler(0, 0, -60f * Time.deltaTime);
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    rot = Quaternion.Euler(0, 0, -20f * Time.deltaTime);
                }
                // 現在の自身の回転の情報を取得する
                this.transform.rotation = myRot * rot;

                isRot = !isRot;
            }

            //角度リセット！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！
            if (Input.GetKey(KeyCode.R))
            {
                this.transform.rotation = Quaternion.Euler(0, 0, 0);
                isRot = !isRot;
            }
        }

        if (VibrateFg)
        {
            VibrateTime += Time.deltaTime;
            if (VibrateTime < 0.4f)
            {
                Vibrate();
            }
            else
            {
                VibrateFg = false;
                VibrateTime = 0.0f;
                transform.position = new Vector3(initPosition, transform.position.y, transform.position.z);
            }
        }
    }

    public void CreateIce()
    {

        //エフェクト発生
        GameObject obj = (GameObject)Resources.Load("test");
        Instantiate(obj, transform.position, Quaternion.identity);


        m_ObjectCollider.isTrigger = false;
        var rb = this.gameObject.AddComponent<Rigidbody2D>();
        this.gameObject.AddComponent<StopIce>();
        rb.mass = 100f;
        rb.sharedMaterial = yuka;

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

        Carver.Carve(thisCarver, carvers);

        //メッシュの生成
        MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();
        Vector3[] test = mf.mesh.vertices;

        Quaternion KeepmyRot = this.transform.rotation;
        this.transform.rotation = Quaternion.Euler(0, 0, 0);
        foreach (Vector3 item in test)
        {
            obj = null;
            obj = (GameObject)Resources.Load("ToumeiSphre");
            Vector3 SphrePos = transform.position;
            obj = Instantiate(obj, transform.position, Quaternion.identity);
            obj.transform.parent = this.transform;
            SphrePos += item;
            obj.transform.position = SphrePos;
            v.Add(item);
        }
        this.transform.rotation = KeepmyRot;

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

        ChangeMaterial.Change();
        //名前付与
        this.gameObject.name = "ice" + num;
        num++;

        foreach (GameObject K_pos in GR_Child)
        {
            Destroy(K_pos);
        }

        Destroy(this);


    }
 
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag =="block")
        {
            //現在接触中のオブジェクトのリストに追加
            TrigeerStayObj.Add(col.gameObject.transform);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "block")
        {
            foreach(Transform item in TrigeerStayObj)
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
   static bool LineSegmentsIntersection(Vector3 p1,Vector3 p2,Vector3 p3,Vector3 p4,out Vector2 intersection)
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

    public bool Mabiki(Vector3 P_Pos)
    {
        myPoint.Clear();
        foreach (GameObject K_pos in GR_Child)
        {
            myPoint.Add(K_pos.transform.position);
        }


        //カットしていいかのフラグ
        bool CutFg = true;
        Vector3 ireru = new Vector3(0, 0, 0);
        Vector3[] watasu = { ireru, ireru, ireru, ireru };

        //マスクボックスの４頂点-------------------------------------------------------
        float mx_pos = MaskCube.transform.position.x;
        float my_pos = MaskCube.transform.position.y;

        float mx_ = (mx_pos + 1.5f);
        float my_ = my_pos + 1.5f;
        watasu[0] = new Vector3(mx_, my_, 1f);

        mx_ = (mx_pos + 1.5f);
        my_ = my_pos - 1.5f;
        watasu[1] = new Vector3(mx_, my_, 1f);

        mx_ = mx_pos - 1.5f;
        my_ = my_pos - 1.5f;
        watasu[2] = new Vector3(mx_, my_, 1f);

        mx_ = (mx_pos - 1.5f);
        my_ = my_pos + 1.5f;
        watasu[3] = new Vector3(mx_, my_, 1f);
        //------------------------------------------------------------------------------

        //マスク内に入ってる頂点の数
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
        Dainyuu.Clear();
        foreach (Vector3 item in myPoint)
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
        for (int i = 0; i < 3; i++)
        {
            if (LineSegmentsIntersection(watasu[1], watasu[2], myPoint[i], myPoint[i + 1], out Kousa))
            {
                if (CheckUpVec(myPoint[i], myPoint[i + 1]))
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
                if (LineSegmentsIntersection(watasu[1], watasu[2], myPoint[3], myPoint[0], out Kousa))
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
        int DamyInNum = InNum;
        if (InNum == 0)
        {
            InNum = 2;
        }
        Vector3[] NewPoint = new Vector3[6 - InNum];
        switch (DamyInNum)
        {
            case 0:

                for (int i = 0; i < 4; i++)
                {
                    NewPoint[i] = myPoint[i];
                }
                break;
            case 1:

                NewPoint[0] = kouten[1];
                NewPoint[1] = kouten[0];

                NewPoint[2] = Dainyuu[0];
                NewPoint[3] = Dainyuu[1];
                NewPoint[4] = Dainyuu[2];

                break;
            case 2:
                NewPoint[0] = kouten[1];
                NewPoint[1] = kouten[0];

                NewPoint[2] = Dainyuu[0];
                NewPoint[3] = Dainyuu[1];
                break;
            case 3:
                NewPoint[0] = kouten[1];
                NewPoint[1] = kouten[0];
                NewPoint[2] = Dainyuu[0];
                break;
            case 4:
                CutFg = false;
                break;
        }

        //トリガーの頂点
        Vector3[] TriPos = { ireru, ireru, ireru, ireru, ireru, ireru, ireru };
        int CrossNum = 0;

        //現在当たっているブロックの頂点で分断されないかのチェック-------------------------------------------------------
        foreach (Transform TriObj in TrigeerStayObj)
        {
            for (int i2 = 1; i2 < TriObj.childCount; i2++)
            {
                TriPos[i2-1] = TriObj.GetChild(i2).position;
            }

            //現在ヒット中のobj
            for (int i = 0; i < TriObj.childCount-2; i++)
            {
                //マスクボックスとflameでできた新しい頂点のfor文
                for (int j = 0; j < 5 - InNum; j++)
                {

                    if (LineSegmentsIntersection(TriPos[i], TriPos[i + 1], NewPoint[j], NewPoint[j + 1], out Kousa))
                    {
                        CrossNum++;
                    }
                    if (j == 4 - InNum)
                    {
                        if (LineSegmentsIntersection(TriPos[i], TriPos[i + 1], NewPoint[j + 1], NewPoint[0], out Kousa))
                        {
                            CrossNum++;
                        }
                    }
                }

                if (i == TriObj.childCount-3)
                {
                    for (int j = 0; j < 5 - InNum; j++)
                    {
                        if (LineSegmentsIntersection(TriPos[i+1], TriPos[0], NewPoint[j], NewPoint[j + 1], out Kousa))
                        {
                            CrossNum++;
                        }
                        if (j == 4 - InNum)
                        {
                            if (LineSegmentsIntersection(TriPos[i+1], TriPos[0], NewPoint[j + 1], NewPoint[0], out Kousa))
                            {
                                CrossNum++;
                            }
                        }
                    }
                }
            }

            if (CrossNum > 3)
            {
                CutFg = false;
                VibrateFg = true;

                this.initPosition = transform.localPosition.x;
                this.newPosition = this.initPosition;
                this.minPosition = this.initPosition - this.vibrateRange;
                this.maxPosition = this.initPosition + this.vibrateRange;
                this.directionToggle = false;
            }
            CrossNum = 0;
        }
        //---------------------------------------------------------------------------------------------------------------
        if (Check(NewPoint, P_Pos, new Vector3(0, 0, 1f)))
        {
            CutFg = false;
            VibrateFg = true;

            this.initPosition = transform.localPosition.x;
            this.newPosition = this.initPosition;
            this.minPosition = this.initPosition - this.vibrateRange;
            this.maxPosition = this.initPosition + this.vibrateRange;
            this.directionToggle = false;
        }
        return CutFg;
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

    private void Vibrate()
    {

        //ポジションが振動幅の範囲を超えた場合、振動方向を切り替える
        if (this.newPosition <= this.minPosition ||
            this.maxPosition <= this.newPosition)
        {
            this.directionToggle = !this.directionToggle;
        }

        //新規ポジションを設定
        this.newPosition = this.directionToggle ?
            this.newPosition + (vibrateSpeed * Time.deltaTime) :
            this.newPosition - (vibrateSpeed * Time.deltaTime);
        this.newPosition = Mathf.Clamp(this.newPosition, this.minPosition, this.maxPosition);

        this.transform.localPosition = new Vector3(this.newPosition, transform.position.y, 1f);
    }

    public void SetYuka(PhysicsMaterial2D mate)
    {
        yuka = mate;
    }
}
