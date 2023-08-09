﻿using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerStats playerStats;
    public Camera mainCamera;
    public MouseLook mouseLook;

    private DoorInteractable doorObject;
    private float mouseDotProduct;
    private float walkDotProduct;

    // Update is called once per frame
    void Update()
    {
        if (doorObject && doorObject.isDoorGrabbed)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0) || !IsPlayerNearby() || doorObject.isSlammed)
            {
                doorObject.ApplyVelocityToDoor(1f);

                mouseLook.isInteracting = false;
                playerStats.canInteract = true;
                doorObject.isDoorGrabbed = false;
                doorObject = null;
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (doorObject && doorObject.isDoorGrabbed)
            {
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    SlamDoor();
                    return;
                }

                doorObject.yDegMotion = doorObject.transform.eulerAngles.y;
                doorObject.lastYDegMotion = doorObject.yDegMotion;
                CalcYDegMotion();

                if (Mathf.Abs(doorObject.motionVelocity) > 0.04f)
                {
                    if (doorObject.prevCoroutine != null)
                    {
                        StopCoroutine(doorObject.prevCoroutine);
                        doorObject.prevCoroutine = null;
                        doorObject.physicsVelocity = 0f;
                    }

                    doorObject.fromRotation = doorObject.transform.rotation;
                    doorObject.toRotation = Quaternion.Euler(doorObject.transform.eulerAngles.x, doorObject.yDegMotion, doorObject.transform.eulerAngles.z);
                    doorObject.transform.rotation = Quaternion.Lerp(doorObject.fromRotation, doorObject.toRotation, 1f);

                    //Calc highest velocity applied by player
                    float calcVelocity = doorObject.motionVelocity * 25f;
                    if (Mathf.Abs(doorObject.physicsVelocity) < Mathf.Abs(calcVelocity) && Mathf.Abs(doorObject.motionVelocity) > 0.12f)
                    {
                        doorObject.physicsVelocity = Mathf.Clamp(calcVelocity, -doorObject.maxOpeningDegrees, doorObject.maxOpeningDegrees);
                    }
                }
                else
                {
                    doorObject.ApplyVelocityToDoor(1f);
                }
            }
            else
            {
                if (playerStats.canInteract && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    GrabDoor();
                }
            }
        }
    }

    //Check if player is near the door
    bool IsPlayerNearby()
    {
        Vector3 doorOrigin = doorObject.transform.position;
        Vector3 doorEdgeRight = doorOrigin + doorObject.transform.right * (doorObject.transform.GetComponent<MeshFilter>().sharedMesh.bounds.size.x - 0.1f);
        Vector3 doorEdgeLeft = doorOrigin - doorObject.transform.right * (doorObject.transform.GetComponent<MeshFilter>().sharedMesh.bounds.size.x - 0.1f);
        bool isPlayerNearby = Physics.CheckCapsule(doorOrigin, doorObject.isLeftSided ? doorEdgeLeft : doorEdgeRight, playerStats.reachDistance - 0.1f, LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);

        return isPlayerNearby;
    }

    //Applies max velocity to close or open the door
    void SlamDoor()
    {
        doorObject.isSlammed = true;

        if (walkDotProduct > 0.02f)
        {
            doorObject.physicsVelocity = doorObject.isLeftSided ? doorObject.maxOpeningDegrees : -doorObject.maxOpeningDegrees;
        }
        else if (walkDotProduct < -0.02f)
        {
            doorObject.physicsVelocity = doorObject.isLeftSided ? -doorObject.maxOpeningDegrees : doorObject.maxOpeningDegrees;
        }
    }

    //Calculates direction of walking applied in relation to door's facing
    float CalcWalkDotProduct()
    {
        Vector3 doorVector = doorObject.transform.forward;
        Vector3 cameraVector = mainCamera.transform.forward;

        return walkDotProduct = Vector3.Dot(cameraVector, doorVector);
    }

    //Calculates direction of mouse movement applied in relation to door's facing
    float CalcMouseDotProduct()
    {
        Vector3 doorVector1 = doorObject.transform.up;
        Vector3 doorVector2 = doorObject.transform.forward;
        Vector3 doorLook = Vector3.Cross(doorVector2, doorVector1);
        Vector3 cameraVector = mainCamera.transform.forward;

        return mouseDotProduct = Vector3.Dot(cameraVector, doorLook);
    }

    void CalcYDegMotion()
    {
        float mouseX = Input.GetAxis("Mouse X") * (mouseLook.mouseSensitivity * doorObject.dragResistanceMouse);
        float mouseY = Input.GetAxis("Mouse Y") * (mouseLook.mouseSensitivity * doorObject.dragResistanceMouse);
        float walkX = Input.GetAxis("Vertical") * doorObject.dragResistanceMouse * Time.deltaTime;
        float walkY = Input.GetAxis("Horizontal") * doorObject.dragResistanceWalk * Time.deltaTime;

        //Set mouse and walk movement based on initial direction
        CalcMouseDotProduct();
        CalcWalkDotProduct();

        //Player movement
        if (playerMovement.IsPlayerMoving())
        {
            if (walkDotProduct < -0.5f)
            {
                //Front Side
                doorObject.yDegMotion = doorObject.ClampRotation(doorObject.isLeftSided ? doorObject.yDegMotion - walkX : doorObject.yDegMotion + walkX);
            }
            else if (walkDotProduct > 0.5f)
            {
                //Back Side
                doorObject.yDegMotion = doorObject.ClampRotation(doorObject.isLeftSided ? doorObject.yDegMotion + walkX : doorObject.yDegMotion - walkX);
            }
            else
            {
                //In between
                doorObject.yDegMotion = doorObject.ClampRotation(doorObject.yDegMotion - walkY);
            }
        }

        //Mouse movement
        mouseY = doorObject.isPlayerColliding && mouseY < 0f ? 0f : mouseY;

        if (walkDotProduct < 0f)
        {
            //Front side
            if (mouseDotProduct < 0.65f && mouseDotProduct > -0.65f)
            {
                //Middle look
                doorObject.yDegMotion = doorObject.ClampRotation(doorObject.isLeftSided ? doorObject.yDegMotion - mouseY : doorObject.yDegMotion + mouseY);
            }
            if (mouseDotProduct > 0.45f || mouseDotProduct < -0.45f)
            {
                //Angled look
                if (mouseDotProduct > 0.45f)
                {
                    mouseX = doorObject.isPlayerColliding && mouseX > 0f ? 0f : mouseX;
                    doorObject.yDegMotion = doorObject.ClampRotation(doorObject.isLeftSided ? doorObject.yDegMotion + mouseX : doorObject.yDegMotion - mouseX);
                }
                else if (mouseDotProduct < -0.45f)
                {
                    mouseX = doorObject.isPlayerColliding && mouseX < 0f ? 0f : mouseX;
                    doorObject.yDegMotion = doorObject.ClampRotation(doorObject.isLeftSided ? doorObject.yDegMotion - mouseX : doorObject.yDegMotion + mouseX);
                }
            }
        }
        else
        {
            //Back side
            if (mouseDotProduct < 0.65f && mouseDotProduct > -0.65f)
            {
                //Middle look
                doorObject.yDegMotion = doorObject.ClampRotation(doorObject.isLeftSided ? doorObject.yDegMotion + mouseY : doorObject.yDegMotion - mouseY);
            }
            if (mouseDotProduct > 0.45f || mouseDotProduct < -0.45f)
            {
                //Angled look
                if (mouseDotProduct > 0.45f)
                {
                    mouseX = doorObject.isPlayerColliding && mouseX < 0f ? 0f : mouseX;
                    doorObject.yDegMotion = doorObject.ClampRotation(doorObject.isLeftSided ? doorObject.yDegMotion + mouseX : doorObject.yDegMotion - mouseX);
                }
                else if (mouseDotProduct < -0.45f)
                {
                    mouseX = doorObject.isPlayerColliding && mouseX > 0f ? 0f : mouseX;
                    doorObject.yDegMotion = doorObject.ClampRotation(doorObject.isLeftSided ? doorObject.yDegMotion - mouseX : doorObject.yDegMotion + mouseX);
                }
            }
        }
    }

    //Check if looking at door and grab it
    void GrabDoor()
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hitInfo, playerStats.reachDistance, LayerMask.GetMask("Door"), QueryTriggerInteraction.Ignore))
        {
            doorObject = hitInfo.transform.gameObject.GetComponent<DoorInteractable>();

            //Play door handle animation
            if (doorObject.doorHandleAnimator != null && doorObject.doorHandleAnimator.GetCurrentAnimatorStateInfo(0).IsName("Default"))
            {
                if (doorObject.IsDoorClosed(0f))
                {
                    switch (doorObject.isLeftSided)
                    {
                        case true:
                            doorObject.doorHandleAnimator.SetTrigger("triggerHandleLeft");
                            break;
                        case false:
                            doorObject.doorHandleAnimator.SetTrigger("triggerHandleRight");
                            break;
                    }
                }
            }

            if (!doorObject.isLocked)
            {
                //Stop any door motion
                if (doorObject.prevCoroutine != null)
                {
                    StopCoroutine(doorObject.prevCoroutine);
                    doorObject.prevCoroutine = null;
                    doorObject.physicsVelocity = 0f;
                }

                mouseLook.isInteracting = true;
                playerStats.canInteract = false;
                doorObject.isSlammed = false;
                doorObject.isDoorGrabbed = true;
            }
            else
            {
                HintUI.instance.DisplayHintMessage("Door is locked!");
                doorObject = null;
            }
        }
    }
}
