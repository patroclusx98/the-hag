using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public Player player;
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
                playerLook.SetObjectTracking(doorObject.GetDoorEdge(DoorInteractable.DoorEdge.Right));
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && player.CanInteract())
            {
                GrabDoor();
            }
        }
    }

    /// <summary>
    /// Sets the facing and angle dot products to determine which side of the door the player is facing 
    /// along with the angle between the player and the doors surface
    /// </summary>
    private void SetDotProducts()
    {
        Vector3 doorForwardVector = doorObject.transform.forward;
        Vector3 doorCrossVector = Vector3.Cross(doorForwardVector, doorObject.transform.up);
        Vector3 playerLookForwardVector = playerLook.transform.forward;

        facingDotProduct = Vector3.Dot(playerLookForwardVector, doorForwardVector);
        angleDotProduct = Vector3.Dot(playerLookForwardVector, doorCrossVector);
    }

    /// <summary>
    /// Sets the doors's Y rotation based on mouse inputs and the player's facing towards the door
    /// </summary>
    private void SetYRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") / doorObject.movementResistance;
        float mouseY = Input.GetAxis("Mouse Y") / doorObject.movementResistance;
        float motionChange = 0f;

        SetDotProducts();

        /** Forwards and backwards mouse movements **/
        if (Mathf.Abs(angleDotProduct) < 0.8f)
        {
            motionChange += facingDotProduct < 0f ? mouseY : -mouseY;
        }

        /** Side to side mouse movements **/
        if (angleDotProduct > 0.4f)
        {
            motionChange += -mouseX;
        }
        else if (angleDotProduct < -0.4f)
        {
            motionChange += mouseX;
        }

        /** Restrict movement towards the player if the player is colliding with the door **/
        if (doorObject.isPlayerColliding)
        {
            motionChange = (facingDotProduct < 0f && motionChange < 0f) ? 0f : (facingDotProduct > 0f && motionChange > 0f) ? 0f : motionChange;
        }

        doorObject.yRotation += doorObject.isLeftSided ? -motionChange : motionChange;
    }

    /// <summary>
    /// Attempts to grab the door the player is looking at
    /// </summary>
    private void GrabDoor()
    {
        bool rayHit = Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out RaycastHit hitInfo, player.reachDistance, LayerMask.GetMask("Door"), QueryTriggerInteraction.Ignore);

        if (rayHit)
        {
            doorObject = hitInfo.transform.gameObject.GetComponent<DoorInteractable>();

            if (!doorObject.isLocked)
            {
                player.modifiers[Player.Modifier.Interacting] = Player.Interaction.Door;
                doorObject.isDoorGrabbed = true;

                doorObject.PlayDoorHandleAnimation();
            }
            else
            {
                HintUI.instance.DisplayHintMessage("Door is locked!");
                doorObject = null;
            }
        }
    }

    /// <summary>
    /// Checks if the door should be let go based on certain conditions
    /// </summary>
    /// <returns>True if the door should be let go</returns>
    private bool ShouldLetGoOfDoor()
    {
        float playerDoorLeftDistance = Vector3.Distance(playerLook.transform.position, doorObject.GetDoorEdge(DoorInteractable.DoorEdge.Left));
        float playerDoorRightDistance = Vector3.Distance(playerLook.transform.position, doorObject.GetDoorEdge(DoorInteractable.DoorEdge.Right));

        /** Player is too far from the door **/
        if (playerDoorLeftDistance > player.reachDistance + 0.1f && playerDoorRightDistance > player.reachDistance + 0.1f)
        {
            return true;
        }

        /** Player can no longer interact with the door **/
        if (!player.CanInteractWith(Player.Interaction.Door))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Lets go of the door the player is currently interacting with
    /// </summary>
    private void LetGoOfDoor()
    {
        if (player.CanEndInteractionWith(Player.Interaction.Door))
        {
            player.modifiers.Remove(Player.Modifier.Interacting);
        }

        playerLook.ResetObjectTracking();

        doorObject.isDoorGrabbed = false;
        doorObject = null;
    }
}
