using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetVtx : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Mesh myMesh = GetComponent<MeshFilter>().mesh;
        for(int i = 0; i < myMesh.vertices.Length; i++){
            Debug.Log(myMesh.vertices[i]);
        }
    }
}
