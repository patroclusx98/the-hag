using UnityEngine;

public class WindowInteraction : MonoBehaviour
{
    public PlayerStats playerStats;
    public Camera mainCamera;
    public MouseLook mouseLook;

    private WindowInteractable windowObject;

    // Update is called once per frame
    private void Update()
    {
        if (windowObject && windowObject.isWindowGrabbed)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0) || !IsPlayerNearby())
            {
                mouseLook.isInteracting = false;
                playerStats.canInteract = true;
                windowObject.isWindowGrabbed = false;
                windowObject = null;
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (windowObject && windowObject.isWindowGrabbed)
            {
                windowObject.yPosMotion = windowObject.transform.position.y;
                windowObject.lastYPosMotion = windowObject.yPosMotion;
                CalcYPosMotion();

                if (windowObject.lastYPosMotion != windowObject.yPosMotion)
                {
                    windowObject.fromPosition = windowObject.transform.position;
                    windowObject.toPosition = new Vector3(windowObject.fromPosition.x, windowObject.yPosMotion, windowObject.fromPosition.z);
                    windowObject.transform.position = Vector3.MoveTowards(windowObject.fromPosition, windowObject.toPosition, 1f);
                }
            }
            else
            {
                if (playerStats.canInteract && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    GrabWindow();
                }
            }
        }
    }

    //Check if player is near the window
    private bool IsPlayerNearby()
    {
        Vector3 windowOrigin = windowObject.transform.position;
        Vector3 windowEdgeBottom = windowOrigin - windowObject.transform.up * (windowObject.transform.GetComponent<MeshFilter>().sharedMesh.bounds.size.y - 0.1f);
        bool isPlayerNearby = Physics.CheckCapsule(windowOrigin, windowEdgeBottom, playerStats.reachDistance - 0.1f, LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);

        return isPlayerNearby;
    }

    private void CalcYPosMotion()
    {
        float mouseY = Input.GetAxis("Mouse Y") * windowObject.dragResistance;
        mouseY = windowObject.isPlayerColliding && mouseY < 0f ? 0f : mouseY;

        windowObject.yPosMotion = Mathf.Clamp(windowObject.yPosMotion + mouseY, windowObject.defaultClosedPosition.y, windowObject.defaultClosedPosition.y + windowObject.maxOpeningPosition);
    }

    //Check if looking at window and grab it
    private void GrabWindow()
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hitInfo, playerStats.reachDistance, LayerMask.GetMask("Window"), QueryTriggerInteraction.Ignore))
        {
            windowObject = hitInfo.transform.gameObject.GetComponent<WindowInteractable>();

            if (!windowObject.isLocked)
            {
                mouseLook.isInteracting = true;
                playerStats.canInteract = false;
                windowObject.isWindowGrabbed = true;
            }
            else
            {
                HintUI.instance.DisplayHintMessage("Window is locked!");
                windowObject = null;
            }

        }
    }
}
