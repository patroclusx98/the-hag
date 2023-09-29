using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerStats playerStats;
    public PlayerLook playerLook;

    private DoorInteractable doorObject;
    private float facingDotProduct;
    private float angleDotProduct;

    // Update is called once per frame
    private void Update()
    {
        if (doorObject && doorObject.isDoorGrabbed)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0) || ShouldLetGoOfDoor())
            {
                LetGoOfDoor();
            }
            else if (Input.GetKey(KeyCode.Mouse0))
            {
                SetYRotation();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && playerStats.canInteract)
            {
                GrabDoor();
            }
        }
    }

    private void SetDotProducts()
    {
        Vector3 doorForwardVector = doorObject.transform.forward;
        Vector3 doorCrossVector = Vector3.Cross(doorForwardVector, doorObject.transform.up);
        Vector3 playerLookForwardVector = playerLook.transform.forward;

        facingDotProduct = Vector3.Dot(playerLookForwardVector, doorForwardVector);
        angleDotProduct = Vector3.Dot(playerLookForwardVector, doorCrossVector);
    }

    private void SetYRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") / doorObject.movementResistance;
        float mouseY = Input.GetAxis("Mouse Y") / doorObject.movementResistance;
        float motionChange = 0f;

        SetDotProducts();

        if (Mathf.Abs(angleDotProduct) < 0.8f)
        {
            motionChange += facingDotProduct < 0f ? mouseY : -mouseY;
        }

        if (angleDotProduct > 0.4f)
        {
            motionChange += -mouseX;
        }
        else if (angleDotProduct < -0.4f)
        {
            motionChange += mouseX;
        }

        if (doorObject.isPlayerColliding)
        {
            motionChange = (facingDotProduct < 0f && motionChange < 0f) ? 0f : (facingDotProduct > 0f && motionChange > 0f) ? 0f : motionChange;
        }

        doorObject.yRotation += doorObject.isLeftSided ? -motionChange : motionChange;
        playerLook.SetObjectTracking(doorObject.GetDoorEdge(DoorInteractable.DoorEdge.Right));
    }

    // Check if looking at door and grab it
    private void GrabDoor()
    {
        bool rayHit = Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out RaycastHit hitInfo, playerStats.reachDistance, LayerMask.GetMask("Door"), QueryTriggerInteraction.Ignore);

        if (rayHit)
        {
            doorObject = hitInfo.transform.gameObject.GetComponent<DoorInteractable>();

            if (!doorObject.isLocked)
            {
                doorObject.PlayDoorHandleAnimation();

                playerStats.canInteract = false;
                doorObject.isDoorGrabbed = true;
            }
            else
            {
                HintUI.instance.DisplayHintMessage("Door is locked!");
                doorObject = null;
            }
        }
    }

    private bool ShouldLetGoOfDoor()
    {
        float playerDoorLeftDistance = Vector3.Distance(playerLook.transform.position, doorObject.GetDoorEdge(DoorInteractable.DoorEdge.Left));
        float playerDoorRightDistance = Vector3.Distance(playerLook.transform.position, doorObject.GetDoorEdge(DoorInteractable.DoorEdge.Right));

        if (playerDoorLeftDistance > playerStats.reachDistance + 0.1f && playerDoorRightDistance > playerStats.reachDistance + 0.1f)
        {
            return true;
        }

        return false;
    }

    private void LetGoOfDoor()
    {
        playerStats.canInteract = true;
        playerLook.ResetObjectTracking();

        doorObject.isDoorGrabbed = false;
        doorObject = null;
    }
}
