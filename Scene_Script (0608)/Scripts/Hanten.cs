using UnityEngine;

public class Hanten : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerControl1 oya;
    void Start()
    {
        oya = transform.root.GetComponent<PlayerControl1>();   
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "block")
        {
            oya.HitChild();
        }
    }
}
