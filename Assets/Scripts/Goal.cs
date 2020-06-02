using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Goal : MonoBehaviour
{
    private List<Vector3> v = new List<Vector3>();
    private DrawMesh dr;
    // Start is called before the first frame update
    void Start()
    {
        dr = this.gameObject.GetComponent<DrawMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Y))
        {
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

            Debug.Log("切り取り完了");

            ////メッシュの生成
            //MeshFilter mf = this.gameObject.GetComponent<MeshFilter>();
            //Vector3[] test = mf.mesh.vertices;


            //foreach (Vector3 item in test)
            //{
            //    v.Add(item);
            //}
            //dr.CreateMesh(v);
            //var child = transform.GetChild(0);

            //子のメッシュを削除
            //var delfilter = child.GetComponent<MeshFilter>();
            //var delrender = child.GetComponent<MeshRenderer>();

            //Destroy(delfilter);
            //Destroy(delrender);
        }
    }
}
