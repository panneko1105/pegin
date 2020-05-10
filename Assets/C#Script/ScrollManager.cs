using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GokUtil.UpdateManager;

public class ScrollManager : MonoBehaviour, IUpdatable
{
    private Renderer rend;
    [SerializeField] private float scrollSpeedX = 0.1f;
    [SerializeField] private float scrollSpeedY = 0.0f;
    //[SerializeField] private bool isAutoScroll = true;    //! 自動スクロールの名残
    [SerializeField] private float div_v = 20.0f;
    private Vector3 oldcampos;
    private Vector3 oldpos;
    private Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        // コンポーーーーーネントの取得
        rend = GetComponent<Renderer>();
        // カメラ情報の取得
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        //cam = Camera.main;
        // 初期位置を覚えておく
        oldpos = transform.position;
        oldcampos = cam.transform.position;
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
        //var x = Mathf.Repeat(Time.time * scrollSpeedX, 1);
        //var y = Mathf.Repeat(Time.time * scrollSpeedY, 1);
        //var offset = new Vector2(x, y);
        //GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
        //float offsetX = Time.time * scrollSpeedX;
        //float offsetY = Time.time * scrollSpeedY;
        //rend.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));

        // テクスチャオフセットをずらす量を算出
        float u = (cam.transform.position.x - oldcampos.x) / div_v;
        float v = (cam.transform.position.y - oldcampos.y) / div_v;
        rend.material.SetTextureOffset("_MainTex", new Vector2(u * scrollSpeedX, v * scrollSpeedY));
    }
}
