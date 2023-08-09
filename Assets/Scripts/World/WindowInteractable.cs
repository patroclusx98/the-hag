using UnityEngine;

public class WindowInteractable : MonoBehaviour
{
    [Header("Window Attributes")]
    public float dragResistance = 0.003f;
    [Range(0.1f, 2f)]
    public float maxOpeningPosition = 1f;
    public bool isLocked = false;
    public Vector3 defaultClosedPosition;

    [Header("Window Inspector Basic")]
    [ReadOnlyInspector]
    public bool isWindowGrabbed = false;
    [ReadOnlyInspector]
    public bool isPlayerColliding = false;

    [Header("Window Inspector Advanced")]
    [ReadOnlyInspector]
    public float yPosMotion;
    [ReadOnlyInspector]
    public float lastYPosMotion;
    [ReadOnlyInspector]
    public Vector3 fromPosition;
    [ReadOnlyInspector]
    public Vector3 toPosition;

    void Reset()
    {
        //Auto set window params
        gameObject.tag = "Interactable";
        gameObject.layer = LayerMask.NameToLayer("Window");
        defaultClosedPosition = gameObject.transform.position;

        //Auto add trigger collider
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        if (gameObject.GetComponent<BoxCollider>() != null)
            DestroyImmediate(boxCollider);
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y + 0.3f, boxCollider.size.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerColliding = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerColliding = false;
        }
    }

    //EVENT HANDLERS FOR OTHER SCRIPTS

    public void EventLockWindow()
    {
        isLocked = true;
    }

    public void EventUnlockWindow()
    {
        isLocked = false;
    }
}
