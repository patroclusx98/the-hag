using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerStats playerStats;

    [Header("Player Look Attributes")]
    [Range(0.3f, 3f)]
    public float mouseSensitivity = 1f;
    public float minHeadXRotation = -70f;
    public float maxHeadXRotation = 85f;
    public float headYRotationLimit = 130f;
    public float backLookSpeed = 15f;
    public float objectTrackSpeed = 8f;

    [Header("Player Look Inspector")]
    [ReadOnlyInspector]
    public bool isMouseLookEnabled = true;
    [ReadOnlyInspector]
    public bool isLookingBack;
    [ReadOnlyInspector]
    public bool isTrackingObject;
    [ReadOnlyInspector]
    public float headXRotation;
    [ReadOnlyInspector]
    public float headYRotation;

    private Quaternion objectTrackingLookRotation;

    // Update is called once per frame
    private void Update()
    {
        if (isMouseLookEnabled)
        {
            MouseLook();
        }
        else if (isTrackingObject)
        {
            TrackObject();
        }
    }

    private void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        if (Input.GetKey(KeyCode.Mouse4))
        {
            isLookingBack = true;
            playerStats.canInteract = false;

            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0f, -headYRotationLimit, 0f), backLookSpeed * Time.deltaTime);

            headXRotation = 0f;
            headYRotation = -(360f - transform.localEulerAngles.y);
        }
        else
        {
            if (!isLookingBack)
            {
                headXRotation = Mathf.Clamp(headXRotation + mouseY, minHeadXRotation, maxHeadXRotation);

                transform.localRotation = Quaternion.Euler(-headXRotation, 0f, 0f);
                playerMovement.transform.Rotate(playerMovement.transform.up * mouseX);
            }
            else
            {
                if (Quaternion.Angle(transform.localRotation, Quaternion.Euler(0f, 0f, 0f)) > 0f)
                {
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0f, 0f, 0f), backLookSpeed * Time.deltaTime);
                    headYRotation = -(360f - transform.localEulerAngles.y);
                }
                else
                {
                    headYRotation = 0f;
                    playerStats.canInteract = true;
                    isLookingBack = false;
                }
            }
        }
    }

    private void TrackObject()
    {
        float objectTrackingLookXRotation = objectTrackingLookRotation.eulerAngles.x;

        if (objectTrackingLookXRotation >= 180f)
        {
            objectTrackingLookXRotation = Mathf.Clamp(360f - objectTrackingLookXRotation, minHeadXRotation, maxHeadXRotation);
        }
        else
        {
            objectTrackingLookXRotation = Mathf.Clamp(-objectTrackingLookXRotation, minHeadXRotation, maxHeadXRotation);
        }

        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(-objectTrackingLookXRotation, 0f, 0f), objectTrackSpeed * Time.deltaTime);
        playerMovement.transform.rotation = Quaternion.Lerp(playerMovement.transform.rotation, Quaternion.Euler(0f, objectTrackingLookRotation.eulerAngles.y, 0f), objectTrackSpeed * Time.deltaTime);

        headXRotation = transform.localEulerAngles.x >= 180f ? 360f - transform.localEulerAngles.x : -transform.localEulerAngles.x;
    }

    public void LockMouseLook()
    {
        isMouseLookEnabled = false;
    }

    public void UnlockMouseLook()
    {
        isMouseLookEnabled = true;
    }

    public void SetObjectTracking(Vector3 objectTrackingPoint)
    {
        objectTrackingLookRotation = Quaternion.LookRotation(objectTrackingPoint - transform.position);
        isMouseLookEnabled = false;
        isTrackingObject = true;
    }

    public void ResetObjectTracking()
    {
        objectTrackingLookRotation = default;
        isMouseLookEnabled = true;
        isTrackingObject = false;
    }
}
