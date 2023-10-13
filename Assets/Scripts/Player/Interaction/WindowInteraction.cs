using UnityEngine;

public class WindowInteraction : MonoBehaviour
{
    public Player player;

    private WindowInteractable windowObject;

    // Update is called once per frame
    private void Update()
    {
        if (windowObject && windowObject.isWindowGrabbed)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0) || ShouldLetGoOfWindow())
            {
                LetGoOfWindow();
            }
            else if (Input.GetKey(KeyCode.Mouse0))
            {
                SetYPosition();
                player.playerLook.SetObjectTracking(windowObject.GetWindowEdge(WindowInteractable.WindowEdge.Bottom));
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && player.CanInteract())
            {
                GrabWindow();
            }
        }
    }

    /// <summary>
    /// Sets the window's Y position based on mouse inputs
    /// </summary>
    private void SetYPosition()
    {
        float mouseY = Input.GetAxis("Mouse Y") / (windowObject.movementResistance * 100f);
        float motionChange = 0f;

        motionChange += mouseY;

        /** Restrict movement towards the player if the player is colliding with the window **/
        if (windowObject.isPlayerColliding)
        {
            motionChange = motionChange < 0f ? 0f : motionChange;
        }

        windowObject.yPosition += motionChange;
    }

    /// <summary>
    /// Attempts to grab the window the player is looking at
    /// </summary>
    private void GrabWindow()
    {
        bool rayHit = Physics.Raycast(player.playerLook.transform.position, player.playerLook.transform.forward, out RaycastHit hitInfo, player.reachDistance, LayerMask.GetMask("Window"), QueryTriggerInteraction.Ignore);

        if (rayHit)
        {
            windowObject = hitInfo.transform.gameObject.GetComponent<WindowInteractable>();

            if (!windowObject.isLocked)
            {
                player.modifiers[Player.Modifier.Interacting] = Player.Interaction.Window;
                windowObject.isWindowGrabbed = true;
            }
            else
            {
                HintUI.instance.DisplayHintMessage("Window is locked!");
                windowObject = null;
            }
        }
    }

    /// <summary>
    /// Checks if the window should be let go based on certain conditions
    /// </summary>
    /// <returns>True if the window should be let go</returns>
    private bool ShouldLetGoOfWindow()
    {
        float playerWindowTopDistance = Vector3.Distance(player.playerLook.transform.position, windowObject.GetWindowEdge(WindowInteractable.WindowEdge.Top));
        float playerWindowBottomDistance = Vector3.Distance(player.playerLook.transform.position, windowObject.GetWindowEdge(WindowInteractable.WindowEdge.Bottom));

        /** Player is too far from the window **/
        if (playerWindowTopDistance > player.reachDistance + 0.1f && playerWindowBottomDistance > player.reachDistance + 0.1f)
        {
            return true;
        }

        /** Player can no longer interact with the window **/
        if (!player.CanInteractWith(Player.Interaction.Window))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Lets go of the window the player is currently interacting with
    /// </summary>
    private void LetGoOfWindow()
    {
        if (player.CanEndInteractionWith(Player.Interaction.Window))
        {
            player.modifiers.Remove(Player.Modifier.Interacting);
        }

        player.playerLook.ResetObjectTracking();

        windowObject.isWindowGrabbed = false;
        windowObject = null;
    }
}
