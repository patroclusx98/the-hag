using System;
using UnityEngine;

public class WindowInteractable : MonoBehaviour
{
    public enum WindowEdge { Top, Bottom }

    [Header("Window Attributes")]
    public Vector3 defaultClosedPosition;
    public Vector3 maxOpeningPosition;
    public float movementResistance = 5f;
    public bool isLocked;

    [Header("Window Inspector Basic")]
    [ReadOnlyInspector]
    public bool isWindowGrabbed;
    [ReadOnlyInspector]
    public bool isPlayerColliding;
    [ReadOnlyInspector]
    public float yPosition;

    // Reset is called on component add/reset
    private void Reset()
    {
        /** Auto set window params **/
        gameObject.tag = "Interactable";
        gameObject.layer = LayerMask.NameToLayer("Window");
        defaultClosedPosition = transform.localPosition;

        /** Auto add/reset trigger collider **/
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        if (boxCollider) DestroyImmediate(boxCollider);
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y + 0.3f, boxCollider.size.z);
    }

    // Start is called before the first frame update
    private void Start()
    {
        yPosition = transform.localPosition.y;
    }

    // Update is called once per frame
    private void Update()
    {
        ClampYPosition();

        Vector3 toPosition = new Vector3(transform.localPosition.x, yPosition, transform.localPosition.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, toPosition, Time.deltaTime * 5f);
    }

    public Vector3 GetWindowEdge(WindowEdge windowEdge)
    {
        switch (windowEdge)
        {
            case WindowEdge.Top:
                {
                    return transform.position + transform.up * (transform.GetComponent<MeshFilter>().sharedMesh.bounds.extents.y - 0.05f);
                }
            default:
            case WindowEdge.Bottom:
                {
                    return transform.position - transform.up * (transform.GetComponent<MeshFilter>().sharedMesh.bounds.extents.y - 0.05f);
                }
        }
    }

    private void ClampYPosition()
    {
        yPosition = Math.Clamp(yPosition, defaultClosedPosition.y, maxOpeningPosition.y);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerColliding = false;
        }
    }
}
