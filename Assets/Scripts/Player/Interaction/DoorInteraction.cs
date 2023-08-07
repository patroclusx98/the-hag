using System.Collections;
using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    private GameObject doorObject;
    [Range(80f, 160f)]
    public float maxOpeningDegrees = 120f;
    public bool isLeftSided;
    public bool isLocked;
    PlayerMovement playerMovement;
    MouseLook mouseLook;
    Animator doorHandleAnimator;

    bool isDoorGrabbed = false;
    bool isSlammed = false;
    bool isPlayerColliding = false;

    float yDegMotion;
    float lastYDegMotion;
    float motionVelocity;
    float physicsVelocity = 0f;
    Quaternion defaultClosedRotation;
    Quaternion fromRotation;
    Quaternion toRotation;
    IEnumerator prevCoroutine;
    float lerpTimer;

    //Vector dot products
    float mouseDotProduct = 0f;
    float walkDotProduct = 0f;

    void Reset()
    {
        //Auto set door params
        gameObject.tag = "Interactable";
        gameObject.layer = LayerMask.NameToLayer("Door");

        //Auto add trigger collider
        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();
        if (gameObject.GetComponent<BoxCollider>() != null)
            DestroyImmediate(boxCollider);
        boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(boxCollider.size.x + 0.1f, boxCollider.size.y, boxCollider.size.z + 0.2f);
    }

    void Start()
    {
        doorObject = gameObject;
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        mouseLook = Camera.main.GetComponent<MouseLook>();
        doorHandleAnimator = doorObject.GetComponentInChildren<Animator>();
        defaultClosedRotation = doorObject.transform.parent.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDoorGrabbed)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0) || !IsPlayerNearby() || isSlammed)
            {
                mouseLook.isInteracting = false;

                ApplyVelocityToDoor(1f);

                isDoorGrabbed = false;
                PlayerStats.canInteract = true;
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (isDoorGrabbed)
            {
                if (Input.GetKey(KeyCode.Mouse1))
                {
                    SlamDoor();
                    return;
                }

                yDegMotion = doorObject.transform.eulerAngles.y;
                lastYDegMotion = yDegMotion;
                CalcYDegMotion();

                if (Mathf.Abs(motionVelocity) > 0.04f)
                {
                    if (prevCoroutine != null)
                    {
                        StopCoroutine(prevCoroutine);
                        prevCoroutine = null;
                        physicsVelocity = 0f;
                    }

                    fromRotation = doorObject.transform.rotation;
                    toRotation = Quaternion.Euler(doorObject.transform.eulerAngles.x, yDegMotion, doorObject.transform.eulerAngles.z);
                    doorObject.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, 1f);

                    //Calc highest velocity applied by player
                    float calcVelocity = motionVelocity * 25f;
                    if (Mathf.Abs(physicsVelocity) < Mathf.Abs(calcVelocity) && Mathf.Abs(motionVelocity) > 0.12f)
                    {
                        physicsVelocity = Mathf.Clamp(calcVelocity, -maxOpeningDegrees, maxOpeningDegrees);
                    }
                }
                else
                {
                    ApplyVelocityToDoor(0.8f);
                }
            }
            else
            {
                if (PlayerStats.canInteract && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    GrabDoor();
                }
            }
        }

        if (!isDoorGrabbed && prevCoroutine == null && !IsDoorClosed(0f) && IsDoorClosed(1.5f))
        {
            CloseDoor();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //Stop any door motion
            if (prevCoroutine != null)
            {
                StopCoroutine(prevCoroutine);
                prevCoroutine = null;
            }
            physicsVelocity = 0f;

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

    //Check if player is near the door
    bool IsPlayerNearby()
    {
        Vector3 doorOrigin = transform.position;
        Vector3 doorEdgeRight = doorOrigin + transform.right * (transform.GetComponent<MeshFilter>().sharedMesh.bounds.size.x - 0.1f);
        Vector3 doorEdgeLeft = doorOrigin - transform.right * (transform.GetComponent<MeshFilter>().sharedMesh.bounds.size.x - 0.1f);
        bool playerNear = Physics.CheckCapsule(doorOrigin, isLeftSided ? doorEdgeLeft : doorEdgeRight, PlayerStats.reachDistance - 0.1f, LayerMask.GetMask("Player"), QueryTriggerInteraction.Ignore);

        return playerNear;
    }

    //Check if door is near to closed position by specified ammount
    //Passing 0f will check for fully closed state
    bool IsDoorClosed(float deviationInDegrees)
    {
        float angle = Quaternion.Angle(doorObject.transform.rotation, defaultClosedRotation);

        if (angle <= deviationInDegrees)
        {
            return true;
        }

        return false;
    }

    //Shuts the door
    void CloseDoor()
    {
        fromRotation = doorObject.transform.rotation;
        toRotation = Quaternion.Euler(doorObject.transform.eulerAngles.x, defaultClosedRotation.eulerAngles.y, doorObject.transform.eulerAngles.z);

        doorObject.transform.rotation = Quaternion.Slerp(fromRotation, toRotation, 0.1f);
    }

    //Clamps rotation to min and max door openings
    float ClampRotation(float rotation)
    {
        //Calculate motion velocity
        float calcDifference = rotation - lastYDegMotion;
        if (calcDifference < -maxOpeningDegrees * 2f)
        {
            calcDifference += 360f;
        }
        else if (calcDifference > maxOpeningDegrees * 2f)
        {
            calcDifference -= 360f;
        }
        motionVelocity = Mathf.Clamp(calcDifference, -2f, 2f);

        //Check and clamp rotation
        if (isLeftSided)
        {
            float fromRotation = defaultClosedRotation.eulerAngles.y;
            float calcMaxRotation = fromRotation - maxOpeningDegrees;
            float toRotation = calcMaxRotation < 0f ? 360f + calcMaxRotation : calcMaxRotation;

            //Negative rotation
            if (fromRotation > toRotation)
            {
                if (rotation <= fromRotation && rotation >= toRotation)
                {
                    return rotation;
                }
                else
                {
                    return Mathf.Clamp(rotation, toRotation, fromRotation);
                }
            }
            else
            {
                float wrappedRotation = rotation < 0f ? 360f + rotation : rotation >= 360f ? rotation - 360f : rotation;
                //ISSUE: R0-LY359(E-359 SE1) || R359+LY1(E360 SE0)

                if (!(wrappedRotation > fromRotation && wrappedRotation < toRotation))
                {
                    return wrappedRotation;
                }
                else if (rotation > fromRotation && motionVelocity > 0f)
                {
                    return fromRotation;
                }
                else if (rotation < toRotation && motionVelocity < 0f)
                {
                    return toRotation;
                }
            }
        }
        else
        {
            float fromRotation = defaultClosedRotation.eulerAngles.y;
            float calcMaxRotation = fromRotation + maxOpeningDegrees;
            float toRotation = calcMaxRotation >= 360f ? calcMaxRotation - 360f : calcMaxRotation;

            //Positive rotation
            if (fromRotation > toRotation)
            {
                float wrappedRotation = rotation < 0f ? 360f + rotation : rotation >= 360f ? rotation - 360f : rotation;

                if (!(wrappedRotation < fromRotation && wrappedRotation > toRotation))
                {
                    return wrappedRotation;
                }
                else if (rotation < fromRotation && motionVelocity < 0f)
                {
                    return fromRotation;
                }
                else if (rotation > toRotation && motionVelocity > 0f)
                {
                    return toRotation;
                }
            }
            else
            {
                if (rotation >= fromRotation && rotation <= toRotation)
                {
                    return rotation;
                }
                else
                {
                    return Mathf.Clamp(rotation, fromRotation, toRotation);
                }
            }
        }

        return rotation;
    }

    //Apply velocity to door
    void ApplyVelocityToDoor(float multiplier)
    {
        if (physicsVelocity != 0f)
        {
            CalcVelocityToRotation(multiplier);

            if (prevCoroutine == null)
            {
                if (isSlammed)
                {
                    prevCoroutine = MoveDoorByVelocity(false);
                }
                else
                {
                    prevCoroutine = MoveDoorByVelocity(true);
                }
                StartCoroutine(prevCoroutine);
            }
        }
    }

    //Calculate the rotation from the velocity
    void CalcVelocityToRotation(float multiplier)
    {
        fromRotation = doorObject.transform.rotation;
        float toRotationYVelocity = ClampRotation(fromRotation.eulerAngles.y + physicsVelocity * multiplier);
        if (toRotationYVelocity != fromRotation.eulerAngles.y)
        {
            toRotation = Quaternion.Euler(doorObject.transform.eulerAngles.x, toRotationYVelocity, doorObject.transform.eulerAngles.z);
        }

        physicsVelocity = 0f;
        lerpTimer = 0f;
    }

    //Moves the door by the applied velocity
    IEnumerator MoveDoorByVelocity(bool useSmoothing)
    {
        while (lerpTimer < 1f)
        {
            lerpTimer += Time.deltaTime / (useSmoothing ? 1.4f : 0.8f);
            doorObject.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, 1 - Mathf.Pow(1 - lerpTimer, 3));

            yield return null;
        }

        isSlammed = false;
        prevCoroutine = null;
    }

    void CalcYDegMotion()
    {
        float sensitivityM = 0.5f;
        float sensitivityW = 60f;
        float mouseX = Input.GetAxis("Mouse X") * (mouseLook.mouseSens * sensitivityM);
        float mouseY = Input.GetAxis("Mouse Y") * (mouseLook.mouseSens * sensitivityM);
        float walkX = Input.GetAxis("Vertical") * sensitivityW * Time.deltaTime;
        float walkY = Input.GetAxis("Horizontal") * sensitivityW * Time.deltaTime;

        //Set mouse and walk movement based on initial direction
        CalcMouseDotProduct();
        CalcWalkDotProduct();

        //Player movement
        if (playerMovement.IsPlayerMoving())
        {
            if (walkDotProduct < -0.5f)
            {
                //Front Side
                yDegMotion = ClampRotation(isLeftSided ? yDegMotion - walkX : yDegMotion + walkX);
            }
            else if (walkDotProduct > 0.5f)
            {
                //Back Side
                yDegMotion = ClampRotation(isLeftSided ? yDegMotion + walkX : yDegMotion - walkX);
            }
            else
            {
                //In between
                yDegMotion = ClampRotation(yDegMotion - walkY);
            }
        }

        //Mouse movement
        mouseY = isPlayerColliding && mouseY < 0f ? 0f : mouseY;

        if (walkDotProduct < 0f)
        {
            //Front side
            if (mouseDotProduct < 0.65f && mouseDotProduct > -0.65f)
            {
                //Middle look
                yDegMotion = ClampRotation(isLeftSided ? yDegMotion - mouseY : yDegMotion + mouseY);
            }
            if (mouseDotProduct > 0.45f || mouseDotProduct < -0.45f)
            {
                //Angled look
                if (mouseDotProduct > 0.45f)
                {
                    mouseX = isPlayerColliding && mouseX > 0f ? 0f : mouseX;
                    yDegMotion = ClampRotation(isLeftSided ? yDegMotion + mouseX : yDegMotion - mouseX);
                }
                else if (mouseDotProduct < -0.45f)
                {
                    mouseX = isPlayerColliding && mouseX < 0f ? 0f : mouseX;
                    yDegMotion = ClampRotation(isLeftSided ? yDegMotion - mouseX : yDegMotion + mouseX);
                }
            }
        }
        else
        {
            //Back side
            if (mouseDotProduct < 0.65f && mouseDotProduct > -0.65f)
            {
                //Middle look
                yDegMotion = ClampRotation(isLeftSided ? yDegMotion + mouseY : yDegMotion - mouseY);
            }
            if (mouseDotProduct > 0.45f || mouseDotProduct < -0.45f)
            {
                //Angled look
                if (mouseDotProduct > 0.45f)
                {
                    mouseX = isPlayerColliding && mouseX < 0f ? 0f : mouseX;
                    yDegMotion = ClampRotation(isLeftSided ? yDegMotion + mouseX : yDegMotion - mouseX);
                }
                else if (mouseDotProduct < -0.45f)
                {
                    mouseX = isPlayerColliding && mouseX > 0f ? 0f : mouseX;
                    yDegMotion = ClampRotation(isLeftSided ? yDegMotion - mouseX : yDegMotion + mouseX);
                }
            }
        }
    }

    //Calculates direction of walking applied in relation to door's facing
    float CalcWalkDotProduct()
    {
        Vector3 doorVector = doorObject.transform.forward;
        Vector3 cameraVector = Camera.main.transform.forward;
        return walkDotProduct = Vector3.Dot(cameraVector, doorVector);
    }

    //Calculates direction of mouse movement applied in relation to door's facing
    float CalcMouseDotProduct()
    {
        Vector3 doorVector1 = doorObject.transform.up;
        Vector3 doorVector2 = doorObject.transform.forward;
        Vector3 doorLook = Vector3.Cross(doorVector2, doorVector1);
        Vector3 cameraVector = Camera.main.transform.forward;
        return mouseDotProduct = Vector3.Dot(cameraVector, doorLook);
    }

    //Check if looking at door and grab it
    void GrabDoor()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitInfo, PlayerStats.reachDistance, LayerMask.GetMask("Door"), QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.transform.gameObject.Equals(doorObject))
            {
                if (!isLocked)
                {
                    //Stop any door motion
                    if (prevCoroutine != null)
                    {
                        StopCoroutine(prevCoroutine);
                        prevCoroutine = null;
                        physicsVelocity = 0f;
                    }

                    //Play door handle animation
                    if (doorHandleAnimator != null)
                    {
                        if (IsDoorClosed(0f))
                        {
                            switch (isLeftSided)
                            {
                                case true:
                                    doorHandleAnimator.SetTrigger("triggerHandleLeft");
                                    break;
                                case false:
                                    doorHandleAnimator.SetTrigger("triggerHandleRight");
                                    break;
                            }
                        }
                    }

                    mouseLook.isInteracting = true;
                    PlayerStats.canInteract = false;
                    isSlammed = false;
                    isDoorGrabbed = true;
                }
                else
                {
                    HintUI.instance.DisplayHintMessage("Door is locked!");
                }
            }
        }
    }

    //Applies max velocity to close or open the door
    void SlamDoor()
    {
        isSlammed = true;

        if (walkDotProduct > 0.02f)
        {
            physicsVelocity = isLeftSided ? maxOpeningDegrees : -maxOpeningDegrees;
        }
        else if (walkDotProduct < -0.02f)
        {
            physicsVelocity = isLeftSided ? -maxOpeningDegrees : maxOpeningDegrees;
        }
    }

    //EVENT HANDLERS FOR OTHER SCRIPTS

    public void EventLockDoor()
    {
        isLocked = true;
    }
    public void EventUnlockDoor()
    {
        isLocked = false;
    }

    public void EventApplyVelocityToDoor(float velocity)
    {
        if (velocity != 0)
        {
            physicsVelocity = velocity;
            ApplyVelocityToDoor(1f);
        }
    }

    public void EventSlamDoor(bool isOpening)
    {
        isSlammed = true;

        if (isOpening)
        {
            physicsVelocity = isLeftSided ? maxOpeningDegrees : -maxOpeningDegrees;
        }
        else
        {
            physicsVelocity = isLeftSided ? maxOpeningDegrees : -maxOpeningDegrees;
        }
    }
}
