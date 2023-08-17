using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerStats playerStats;
    public Camera mainCamera;
    public MouseLook mouseLook;
    public LayerMask ignoredLayer;

    [Header("Interaction Attributes")]
    public float maxObjectCarryWeight = 5f;
    public float maxObjectDragWeight = 15f;

    [Header("Interaction Inspector")]
    [ReadOnlyInspector]
    public bool isObjectGrabbed;
    [ReadOnlyInspector]
    public bool carryingObject;
    [ReadOnlyInspector]
    public bool carryingHeavyObject;

    private GameObject objectInHand;
    private Rigidbody objectInHandRB;

    private float defaultDrag;
    private float defaultAngularDrag;
    private float defaultPlayerWalkSpeed;
    private float objDistanceBySize;
    private Transform defaultParent;
    private Vector3 defaultScale;
    private Vector3 lastPosition;
    private Vector3 lastVelocity;

    // Start is called before the first frame update
    private void Start()
    {
        defaultPlayerWalkSpeed = playerMovement.playerStats.walkSpeed;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isObjectGrabbed)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0) || !carryingObject)
            {
                if (carryingObject)
                {
                    DropObj();
                }

                isObjectGrabbed = false;
                mouseLook.isInteracting = false;
                playerStats.canInteract = true;
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (carryingObject)
            {
                if (objectInHandRB.mass <= maxObjectCarryWeight)
                {
                    CarryObject();

                    /** Object rotation in hand **/
                    if (Input.GetKey(KeyCode.R))
                    {
                        mouseLook.isInteracting = true;

                        float mouseX = Input.GetAxis("Mouse X") * 2f;
                        float mouseY = Input.GetAxis("Mouse Y") * 2f;

                        objectInHand.transform.Rotate(mainCamera.transform.up, -mouseX, Space.World);
                        objectInHand.transform.Rotate(mainCamera.transform.right, mouseY, Space.World);
                    }
                    else if (Input.GetKeyUp(KeyCode.R))
                    {
                        mouseLook.isInteracting = false;
                    }
                }
                else
                {
                    mouseLook.isInteracting = true;
                    DragObject();
                }
            }
            else
            {
                if (playerStats.canInteract && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    PickUpObject();
                }
            }
        }
    }

    private bool CheckIfObjUnderPlayer()
    {
        if (playerMovement.gameObjectUnderPlayer != null && playerMovement.gameObjectUnderPlayer == objectInHand)
        {
            return true;
        }

        return false;
    }

    private void CheckObjDrop()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 cameraFacing = mainCamera.transform.forward;

        // Object too heavy
        if (objectInHandRB.mass > maxObjectCarryWeight && objectInHandRB.mass > maxObjectDragWeight)
        {
            HintUI.instance.DisplayHintMessage("Object is too heavy!");
            DropObj();
        }

        // Object out of hands
        bool rayHit = Physics.Raycast(cameraPosition, cameraFacing, out RaycastHit hitInfo, playerStats.reachDistance, ~ignoredLayer, QueryTriggerInteraction.Ignore);
        if (rayHit && !hitInfo.transform.gameObject.Equals(objectInHand))
        {
            DropObj();
        }

        // Object under player
        if (CheckIfObjUnderPlayer())
        {
            DropObj();
        }
    }

    private Vector3 GetVelocity()
    {
        Vector3 velocity = (objectInHand.transform.position - lastPosition) / Time.deltaTime;
        lastPosition = objectInHand.transform.position;

        return velocity;
    }

    private Vector3 GetVelocityDirection()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        Vector3 xDirection = objectInHand.transform.position + mainCamera.transform.forward + (mainCamera.transform.right * mouseX);
        Vector3 yDirection = objectInHand.transform.position + mainCamera.transform.forward + (mainCamera.transform.up * mouseY);
        Vector3 xyDirection = (xDirection + yDirection) / 2f;

        return xyDirection - objectInHand.transform.position;
    }

    private void StopObjectForces()
    {
        objectInHandRB.velocity = Vector3.zero;
        objectInHandRB.angularVelocity = Vector3.zero;
        objectInHandRB.Sleep();
    }

    private void CenterHandObject()
    {
        Vector3 movementVector = Vector3.MoveTowards(objectInHand.transform.position, gameObject.transform.position, 1f);

        if (Vector3.Distance(objectInHand.transform.position, movementVector) > 0.001f)
        {
            objectInHandRB.drag = 40f;
            objectInHandRB.angularDrag = 40f;
            objectInHandRB.AddForce(9f * objectInHandRB.drag * (movementVector - objectInHand.transform.position), ForceMode.Force);
        }
        else
        {
            objectInHandRB.drag = defaultDrag;
            objectInHandRB.angularDrag = defaultAngularDrag;
            StopObjectForces();
        }
    }

    private void CarryObject()
    {
        // Get object movement directional velocity
        GetVelocityDirection();

        // Try to move object to center of camera
        CenterHandObject();
        lastVelocity = GetVelocity();

        // If object cannot be held anymore drop it
        CheckObjDrop();

        // Throw object away
        if (Input.GetKey(KeyCode.Mouse1))
        {
            DropObj();
            objectInHandRB.AddForce(mainCamera.transform.forward * playerStats.strength);
        }
    }

    private void DragObject()
    {
        /** Move object with player **/

        Vector3 objPosition = objectInHand.transform.position;
        Vector3 toPosition = objPosition + playerMovement.moveVelocity * playerMovement.playerSpeed;

        if (playerMovement.IsPlayerMoving())
        {
            objectInHand.transform.position = Vector3.Lerp(objPosition, toPosition, Time.deltaTime);
        }

        // If object cannot be dragged anymore let go
        CheckObjDrop();

        // Push object away
        if (Input.GetKey(KeyCode.Mouse1))
        {
            DropObj();
            objectInHandRB.AddForce(10f * playerStats.strength * playerMovement.transform.forward);
        }
    }

    private void DropObj()
    {
        // If velocity persists, apply that to the object
        objectInHandRB.AddForce(GetVelocityDirection() * Mathf.Clamp(lastVelocity.magnitude * 2f, 0f, 30f), ForceMode.Force);

        // Reset object back to default
        objectInHand.transform.parent = defaultParent;
        objectInHand.layer = LayerMask.NameToLayer("Object");
        objectInHand.transform.localScale = defaultScale;
        objectInHandRB.drag = defaultDrag;
        objectInHandRB.angularDrag = defaultAngularDrag;
        objectInHandRB.useGravity = true;

        // Reset player hand and stats
        gameObject.transform.position = gameObject.transform.position - gameObject.transform.forward * objDistanceBySize;

        if (carryingHeavyObject)
        {
            playerMovement.playerStats.walkSpeed = defaultPlayerWalkSpeed;
            playerMovement.playerStats.SetCanRun(true);
        }

        carryingObject = false;
        carryingHeavyObject = false;
    }

    private void CalcDistanceBySize()
    {
        Vector3 objBoundsSize = Vector3.Scale(objectInHand.transform.localScale, objectInHand.GetComponent<MeshFilter>().sharedMesh.bounds.size);
        objDistanceBySize = Mathf.Clamp(objBoundsSize.magnitude - 0.75f, 0f, 0.4f);

        gameObject.transform.position = gameObject.transform.position + gameObject.transform.forward * objDistanceBySize;
    }

    // Called once when object is being picked up
    private void PickUpObject()
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hitInfo, playerStats.reachDistance, LayerMask.GetMask("Object"), QueryTriggerInteraction.Ignore))
        {
            objectInHand = hitInfo.transform.gameObject;
            objectInHandRB = objectInHand.GetComponent<Rigidbody>();

            if (objectInHand.CompareTag("Interactable") && objectInHandRB != null)
            {
                if (!CheckIfObjUnderPlayer())
                {
                    /** Get object defaults **/
                    defaultParent = objectInHand.transform.parent;
                    defaultScale = objectInHand.transform.localScale;
                    defaultDrag = objectInHandRB.drag;
                    defaultAngularDrag = objectInHandRB.angularDrag;

                    // Set object params
                    CalcDistanceBySize();

                    if (objectInHandRB.mass <= maxObjectCarryWeight)
                    {
                        objectInHand.transform.parent = gameObject.transform;
                        objectInHandRB.useGravity = false;
                    }
                    else
                    {
                        playerMovement.playerStats.walkSpeed = playerMovement.playerStats.walkSpeed * 0.7f - (1f - 5f / objectInHandRB.mass);
                        playerMovement.playerStats.SetCanRun(false);
                        carryingHeavyObject = true;
                    }

                    StopObjectForces();
                    objectInHand.layer = LayerMask.NameToLayer("ObjectCarried");

                    carryingObject = true;
                    isObjectGrabbed = true;
                    playerStats.canInteract = false;
                }
            }
        }
    }
}
