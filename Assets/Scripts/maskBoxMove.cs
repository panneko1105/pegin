using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.VFX;
using GokUtil.UpdateManager;


public class maskBoxMove : MonoBehaviour, IUpdatable
{
    public float move = 5.0f;
    [HideInInspector] public int flg = 0;
    [HideInInspector] public bool isRot = false;        //trueで傾いている
    private List<Vector3> v = new List<Vector3>();

    private int fase;

    PolygonCollider2D m_ObjectCollider;

    private GameObject _child;

    private DrawMesh dr;
    // Start is called before the first frame update
    void Start()
    {
        m_ObjectCollider = GetComponent<PolygonCollider2D>();
        fase = 4;

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
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(0, move * Time.deltaTime, 0);
        }
        // 下移動
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(0, -move * Time.deltaTime, 0);
        }
        // 左に移動
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(move * Time.deltaTime, 0, 0);
        }
        // 右に移動
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-move * Time.deltaTime, 0, 0);
        }
       
        if (Input.GetKey(KeyCode.UpArrow))
        {
            var ke = transform.position;
            ke.y += 1.5f * Time.deltaTime;
            transform.position = ke;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            var ke = transform.position;
            ke.y -= 1.5f * Time.deltaTime;
            transform.position = ke;
        }
    }
}
