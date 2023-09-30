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
        /** Automatically set game object parameters **/
        gameObject.tag = "Interactable";
        gameObject.layer = LayerMask.NameToLayer("Window");
        defaultClosedPosition = transform.localPosition;

        /** Automatically add a trigger collider component to the game object **/
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

        /** Translate game object's Y position to new Y position **/
        Vector3 toPosition = new Vector3(transform.localPosition.x, yPosition, transform.localPosition.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, toPosition, Time.deltaTime * 5f);
    }

    /// <summary>
    /// Calculates and returns the position of the specified edge of the window
    /// It is offset from the window's origin position
    /// </summary>
    /// <param name="windowEdge">The edge to return</param>
    /// <returns>Vector3 of the edge position</returns>
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

    /// <summary>
    /// Clamps the Y position of the window to it's defined min and max Y positions
    /// </summary>
    private void ClampYPosition()
    {
        yPosition = Math.Clamp(yPosition, defaultClosedPosition.y, maxOpeningPosition.y);
    }

    /// <summary>
    /// Runs when the player's collider enters the window's trigger collider
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerColliding = true;
        }
    }


    /// <summary>
    /// Runs when the player's collider leaves the window's trigger collider
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerColliding = false;
        }
    }
}
