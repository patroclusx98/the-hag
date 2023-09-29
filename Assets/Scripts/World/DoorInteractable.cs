using UnityEngine;

public class DoorInteractable : MonoBehaviour
{
    public enum DoorEdge { Left, Right }
    public Animator doorHandleAnimator;

    [Header("Door Attributes")]
    public Quaternion defaultClosedRotation;
    public Quaternion maxOpeningRotation;
    public float movementResistance = 3f;
    public bool isLeftSided;
    public bool isLocked;

    [Header("Door Inspector")]
    [ReadOnlyInspector]
    public bool isDoorGrabbed;
    [ReadOnlyInspector]
    public bool isPlayerColliding;
    [ReadOnlyInspector]
    public float yRotation;

    // Reset is called on component add/reset
    private void Reset()
    {
        /** Auto set door params **/
        gameObject.tag = "Interactable";
        gameObject.layer = LayerMask.NameToLayer("Door");
        defaultClosedRotation = transform.localRotation;

        /** Auto add/reset trigger collider **/
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        if (boxCollider) DestroyImmediate(boxCollider);
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(boxCollider.size.x + 0.1f, boxCollider.size.y + 0.1f, boxCollider.size.z + 0.2f);
    }

    // Start is called before the first frame update
    private void Start()
    {
        yRotation = transform.localEulerAngles.y;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isDoorGrabbed || (!isDoorGrabbed && GetAnglesToClosedRotation() > 0.6f))
        {
            ClampYRotation();

            Quaternion toRotation = Quaternion.Euler(transform.localEulerAngles.x, yRotation, transform.localEulerAngles.z);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, toRotation, Time.deltaTime * 2f);
        }

        /** Auto close door if slightly open **/
        if (!isDoorGrabbed && !IsDoorClosed() && GetAnglesToClosedRotation() <= 0.6f)
        {
            yRotation = defaultClosedRotation.eulerAngles.y;
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, defaultClosedRotation, Time.deltaTime * 2f);
        }
    }

    public Vector3 GetDoorEdge(DoorEdge doorEdge)
    {
        switch (doorEdge)
        {
            case DoorEdge.Left:
                {
                    return transform.position;
                }
            default:
            case DoorEdge.Right:
                {
                    Vector3 doorRightVector = isLeftSided ? -transform.right : transform.right;
                    return transform.position + doorRightVector * (transform.GetComponent<MeshFilter>().sharedMesh.bounds.size.x - 0.1f);
                }
        }
    }

    /// <summary>
    /// Checks if the door is fully closed
    /// </summary>
    /// <returns>True if fully closed</returns>
    public bool IsDoorClosed()
    {
        return transform.localEulerAngles.y == defaultClosedRotation.eulerAngles.y;
    }

    /// <summary>
    /// Calculates the angle between current rotation and closed rotation
    /// </summary>
    /// <returns>Angle between current rotation and closed rotation</returns>
    public float GetAnglesToClosedRotation()
    {
        return Quaternion.Angle(transform.localRotation, defaultClosedRotation);
    }

    // Clamps rotation to min and max door openings
    private void ClampYRotation()
    {
        Quaternion toRotation = Quaternion.Euler(transform.localEulerAngles.x, yRotation, transform.localEulerAngles.z);
        float maxAngleDifference = Quaternion.Angle(defaultClosedRotation, maxOpeningRotation);
        float closingAngle = Quaternion.Angle(transform.localRotation, maxOpeningRotation);
        float openingAngle = Quaternion.Angle(toRotation, defaultClosedRotation);

        if (closingAngle > maxAngleDifference)
        {
            yRotation = defaultClosedRotation.eulerAngles.y;
            transform.localRotation = defaultClosedRotation;
        }
        else if (openingAngle > maxAngleDifference)
        {
            yRotation = maxOpeningRotation.eulerAngles.y;
        }
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

    public void PlayDoorHandleAnimation()
    {
        if (doorHandleAnimator != null && IsDoorClosed())
        {
            if (isLeftSided && !doorHandleAnimator.GetBool("UseLeftHandle"))
            {
                doorHandleAnimator.SetBool("UseLeftHandle", true);
            }
            else if (!isLeftSided && !doorHandleAnimator.GetBool("UseRightHandle"))
            {
                doorHandleAnimator.SetBool("UseRightHandle", true);
            }
        }
    }
}
