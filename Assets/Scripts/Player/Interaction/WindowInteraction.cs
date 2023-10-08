using UnityEngine;

public class WindowInteraction : MonoBehaviour
{
    public Player player;
    public PlayerLook playerLook;

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

    private void SetYPosition()
    {
        float mouseY = Input.GetAxis("Mouse Y") / (windowObject.movementResistance * 100f);
        float motionChange = 0f;

        motionChange += mouseY;

        if (windowObject.isPlayerColliding)
        {
            motionChange = motionChange < 0f ? 0f : motionChange;
        }

        windowObject.yPosition += motionChange;
        playerLook.SetObjectTracking(windowObject.GetWindowEdge(WindowInteractable.WindowEdge.Bottom));
    }

    private void GrabWindow()
    {
        bool rayHit = Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out RaycastHit hitInfo, player.reachDistance, LayerMask.GetMask("Window"), QueryTriggerInteraction.Ignore);

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

    private bool ShouldLetGoOfWindow()
    {
        float playerWindowTopDistance = Vector3.Distance(playerLook.transform.position, windowObject.GetWindowEdge(WindowInteractable.WindowEdge.Top));
        float playerWindowBottomDistance = Vector3.Distance(playerLook.transform.position, windowObject.GetWindowEdge(WindowInteractable.WindowEdge.Bottom));

        if (playerWindowTopDistance > player.reachDistance + 0.1f && playerWindowBottomDistance > player.reachDistance + 0.1f)
        {
            return true;
        }

        if (!player.CanInteractWith(Player.Interaction.Window))
        {
            return true;
        }

        return false;
    }

    private void LetGoOfWindow()
    {
        if (player.CanEndInteractionWith(Player.Interaction.Window))
        {
            player.modifiers.Remove(Player.Modifier.Interacting);
        }

        playerLook.ResetObjectTracking();

        windowObject.isWindowGrabbed = false;
        windowObject = null;
    }
}
