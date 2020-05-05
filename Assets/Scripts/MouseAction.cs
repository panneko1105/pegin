using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MouseAction : MonoBehaviour
{
    private Camera mainCamera;
    private Transform dragTarget;
    private Vector3 dragTargetOrigin;
    private Vector3 dragOrigin;
    private bool carveWhenDrop;

    private void Start()
    {
        this.mainCamera = Camera.main;
    }

    private void Update()
    {
        if (this.dragTarget == null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mouseRay = this.mainCamera.ScreenPointToRay(Input.mousePosition);
                var hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
                if (hit.collider != null)
                {
                    this.dragTarget = hit.transform;
                    this.dragTargetOrigin = this.dragTarget.position;
                    this.dragOrigin = hit.point;
                    if (Input.GetKey(KeyCode.Space))
                    {
                        hit.collider.isTrigger = true;
                        if (hit.rigidbody != null)
                        {
                            hit.rigidbody.bodyType = RigidbodyType2D.Kinematic;
                        }

                        this.carveWhenDrop = true;
                    }
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                var mouseRay = this.mainCamera.ScreenPointToRay(Input.mousePosition);
                var xyPlane = new Plane(Vector3.back, Vector3.zero);
                if (xyPlane.Raycast(mouseRay, out var enter))
                {
                    var deltaPosition = mouseRay.GetPoint(enter) - this.dragOrigin;
                    this.dragTarget.position = this.dragTargetOrigin + deltaPosition;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                var targetRigidbody = this.dragTarget.GetComponent<Rigidbody2D>();
                if (targetRigidbody != null)
                {
                    targetRigidbody.velocity = Vector2.zero;
                    if (this.carveWhenDrop)
                    {
                        targetRigidbody.bodyType = RigidbodyType2D.Dynamic;
                    }
                }

                if (this.carveWhenDrop)
                {
                    var targetCollider = this.dragTarget.GetComponent<Collider2D>();
                    if (targetCollider != null)
                    {
                        targetCollider.isTrigger = false;
                        var overlappingColliders = new List<Collider2D>();
                        targetCollider.OverlapCollider(new ContactFilter2D(), overlappingColliders);
                        var carvers = overlappingColliders.Select(c => c.GetComponentInChildren<Carver>())
                            .Where(c => c != null);
                        var thisCarver = targetCollider.GetComponentInChildren<Carver>();
                        if (thisCarver != null)
                        {
                            Debug.Log(
                                $"Carve {targetCollider.name} with {string.Join(", ", carvers.Select(c => c.Collider2D.name))}.");
                            Carver.Carve(thisCarver, carvers);
                        }
                    }

                    this.carveWhenDrop = false;
                }

                this.dragTarget = null;
            }
        }
    }

    private void OnGUI()
    {
        if (this.carveWhenDrop)
        {
            using (new GUILayout.AreaScope(new Rect(0, 0, Screen.width, Screen.height)))
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Let's Carve!");
                    GUILayout.FlexibleSpace();
                }
            }
        }
    }
}