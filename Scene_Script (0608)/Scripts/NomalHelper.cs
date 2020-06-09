using UnityEngine;

public class NomalHelper : MonoBehaviour
{
    void Start()
    {
        var rb = this.GetComponent<Rigidbody2D>();
        var mate = rb.sharedMaterial;

        mate.friction = 0.1f;
        rb.sharedMaterial = mate;
    }
}
