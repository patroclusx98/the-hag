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
    public bool isLookingBackwards;
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

    /// <summary>
    /// Rotates the player's head and body based on mouse inputs
    /// </summary>
    private void MouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        if (Input.GetKey(KeyCode.Mouse4))
        {
            /** Player is looking backwards **/

            isLookingBackwards = true;
            playerStats.canInteract = false;

            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0f, -headYRotationLimit, 0f), backLookSpeed * Time.deltaTime);

            headXRotation = 0f;
            headYRotation = -(360f - transform.localEulerAngles.y);
        }
        else
        {
            if (!isLookingBackwards)
            {
                /** Player is looking forwards **/

                headXRotation = Mathf.Clamp(headXRotation + mouseY, minHeadXRotation, maxHeadXRotation);

                transform.localRotation = Quaternion.Euler(-headXRotation, 0f, 0f);
                playerMovement.transform.Rotate(playerMovement.transform.up * mouseX);
            }
            else
            {
                /** Player is returning to look forwards **/

                if (Quaternion.Angle(transform.localRotation, Quaternion.Euler(0f, 0f, 0f)) > 0f)
                {
                    transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0f, 0f, 0f), backLookSpeed * Time.deltaTime);
                    headYRotation = -(360f - transform.localEulerAngles.y);
                }
                else
                {
                    headYRotation = 0f;
                    playerStats.canInteract = true;
                    isLookingBackwards = false;
                }
            }
        }
    }

    /// <summary>
    /// Rotates the player's head and body to constantly focus in the given look rotation
    /// </summary>
    private void TrackObject()
    {
        float objectTrackingLookXRotation = objectTrackingLookRotation.eulerAngles.x;

        /** Clamp head's X rotation **/
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

    /// <summary>
    /// Locks mouse input based player rotations
    /// </summary>
    public void LockMouseLook()
    {
        isMouseLookEnabled = false;
    }

    /// <summary>
    /// Unlocks mouse input based player rotations
    /// </summary>
    public void UnlockMouseLook()
    {
        isMouseLookEnabled = true;
    }

    /// <summary>
    /// Sets the tracking look rotation based on the given tracking point
    /// <para>This locks mouse input based player rotations</para>
    /// </summary>
    /// <param name="objectTrackingPoint">The object's point to track</param>
    public void SetObjectTracking(Vector3 objectTrackingPoint)
    {
        objectTrackingLookRotation = Quaternion.LookRotation(objectTrackingPoint - transform.position);
        isMouseLookEnabled = false;
        isTrackingObject = true;
    }

    /// <summary>
    /// Resets the tracking look rotation
    /// <para>This unlocks mouse input based player rotations</para>
    /// </summary>
    public void ResetObjectTracking()
    {
        objectTrackingLookRotation = default;
        isMouseLookEnabled = true;
        isTrackingObject = false;
    }
}
