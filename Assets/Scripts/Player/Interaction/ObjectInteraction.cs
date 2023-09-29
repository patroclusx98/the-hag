using System;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerStats playerStats;
    public PlayerLook playerLook;

    [Header("Interaction Attributes")]
    public float maxObjectCarryWeight = 5f;
    public float maxObjectDragWeight = 15f;

    [Header("Interaction Inspector")]
    [ReadOnlyInspector]
    public bool isObjectGrabbed;
    [ReadOnlyInspector]
    public bool isObjectWithinReach;

    private GameObject objectInHand;
    private Rigidbody objectRBInHand;

    private Transform defaultObjectParent;
    private float defaultObjectDrag;
    private float defaultObjectAngularDrag;
    private float defaultPlayerWalkSpeed;

    private Vector3 lastObjectPosition;
    private Vector3 lastObjectVelocity;

    // Start is called before the first frame update
    private void Start()
    {
        defaultPlayerWalkSpeed = playerStats.walkSpeed;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isObjectGrabbed)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0) || ShouldDropObject())
            {
                DropObject();
            }
            else if (Input.GetKey(KeyCode.Mouse0))
            {
                if (objectRBInHand.mass <= maxObjectCarryWeight)
                {
                    CarryObject();
                }
                else
                {
                    DragObject();
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && playerStats.canInteract)
            {
                PickUpObject();
            }
        }
    }

    private bool IsObjectUnderPlayer()
    {
        return playerMovement.gameObjectUnderPlayer == objectInHand;
    }

    private void StopObjectForces()
    {
        objectRBInHand.velocity = Vector3.zero;
        objectRBInHand.angularVelocity = Vector3.zero;
        objectRBInHand.Sleep();
    }

    private void SetLastObjectVelocity()
    {
        float mouseXY = Math.Abs(Input.GetAxis("Mouse X")) + Math.Abs(Input.GetAxis("Mouse Y"));
        float forceMultiplier = Mathf.Clamp(objectRBInHand.mass, 1f, maxObjectCarryWeight);
        Vector3 throwDirection = Mathf.Clamp(mouseXY, 1f, 5f) * (objectInHand.transform.position - lastObjectPosition);

        lastObjectVelocity = playerStats.strength * forceMultiplier * throwDirection;
        lastObjectPosition = objectInHand.transform.position;
    }

    private void CenterObjectToHand()
    {
        if (Vector3.Distance(objectInHand.transform.position, transform.position) > 0.1f)
        {
            float forceMultiplier = Mathf.Clamp(objectRBInHand.mass, 1f, maxObjectCarryWeight);

            objectRBInHand.drag = 40f;
            objectRBInHand.angularDrag = 40f;
            objectRBInHand.AddForce(objectRBInHand.drag * forceMultiplier * (transform.position - objectInHand.transform.position), ForceMode.Force);
        }
        else
        {
            objectRBInHand.drag = defaultObjectDrag;
            objectRBInHand.angularDrag = defaultObjectAngularDrag;
            StopObjectForces();
            SetLastObjectVelocity();
        }
    }

    private void CarryObject()
    {
        CenterObjectToHand();

        /** Rotate object in hand **/
        if (Input.GetKey(KeyCode.R))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            objectInHand.transform.Rotate(transform.up, -mouseX, Space.World);
            objectInHand.transform.Rotate(transform.right, mouseY, Space.World);

            playerLook.LockMouseLook();
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            playerLook.UnlockMouseLook();
        }

        /** Throw object away **/
        if (Input.GetKey(KeyCode.Mouse1))
        {
            float forceMultiplier = Mathf.Clamp(objectRBInHand.mass, 1f, maxObjectCarryWeight);
            Vector3 throwDirection = playerLook.transform.forward;

            lastObjectVelocity = playerStats.strength * forceMultiplier * throwDirection;
            DropObject();
        }
    }

    private void DragObject()
    {
        /** Move object with player **/
        if (playerMovement.IsPlayerMoving())
        {
            Vector3 objectPosition = objectInHand.transform.position;
            Vector3 toPosition = objectPosition + playerMovement.horizontalVelocity * playerMovement.playerSpeed;

            objectInHand.transform.position = Vector3.Lerp(objectPosition, toPosition, Time.deltaTime);
        }

        playerLook.SetObjectTracking(objectInHand.transform.position);

        /** Push object away **/
        if (Input.GetKey(KeyCode.Mouse1))
        {
            float forceMultiplier = Mathf.Clamp(objectRBInHand.mass, 1f, maxObjectDragWeight);
            Vector3 pushDirection = Quaternion.AngleAxis(90f, Vector3.up) * Vector3.Cross(playerLook.transform.forward, playerLook.transform.up);

            lastObjectVelocity = playerStats.strength * forceMultiplier * pushDirection;
            DropObject();
        }
    }

    // Called once when object is being picked up
    private void PickUpObject()
    {
        bool rayHit = Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out RaycastHit hitInfo, playerStats.reachDistance, LayerMask.GetMask("Object"), QueryTriggerInteraction.Ignore);

        if (rayHit)
        {
            objectInHand = hitInfo.transform.gameObject;
            objectRBInHand = objectInHand.GetComponent<Rigidbody>();

            if (objectRBInHand != null)
            {
                if (objectRBInHand.mass <= maxObjectDragWeight && !IsObjectUnderPlayer())
                {
                    defaultObjectParent = objectInHand.transform.parent;
                    defaultObjectDrag = objectRBInHand.drag;
                    defaultObjectAngularDrag = objectRBInHand.angularDrag;

                    if (objectRBInHand.mass <= maxObjectCarryWeight)
                    {
                        objectInHand.transform.parent = transform;
                        objectRBInHand.useGravity = false;
                    }
                    else
                    {
                        playerStats.walkSpeed = playerStats.walkSpeed * maxObjectCarryWeight / objectRBInHand.mass;
                        playerStats.SetCanJump(false);
                    }

                    playerStats.SetCanRun(false);
                    playerStats.canInteract = false;

                    StopObjectForces();
                    isObjectGrabbed = true;
                }
                else
                {
                    HintUI.instance.DisplayHintMessage("Object is too heavy!");
                    objectInHand = null;
                    objectRBInHand = null;
                }
            }
            else
            {
                Debug.LogWarning("Interactable Object does not contain Rigidbody component: " + objectInHand.name);
                objectInHand = null;
            }
        }
    }

    private bool ShouldDropObject()
    {
        // Object is out of reach
        bool rayHit1 = Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out RaycastHit hitInfo1, playerStats.reachDistance, LayerMask.GetMask("Object"), QueryTriggerInteraction.Ignore);
        if ((!rayHit1 || hitInfo1.transform.gameObject != objectInHand) && !isObjectWithinReach)
        {
            return true;
        }

        // Obstruction between hand and object
        bool rayHit2 = Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out RaycastHit hitInfo2, playerStats.reachDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        if (rayHit2 && hitInfo2.transform.gameObject != objectInHand)
        {
            return true;
        }

        // Object is under the player
        if (IsObjectUnderPlayer())
        {
            return true;
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        isObjectWithinReach = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isObjectWithinReach = false;
    }

    private void DropObject()
    {
        // Reset object back to defaults
        objectInHand.transform.parent = defaultObjectParent;
        objectRBInHand.drag = defaultObjectDrag;
        objectRBInHand.angularDrag = defaultObjectAngularDrag;
        objectRBInHand.useGravity = true;

        // Apply any velocity to the object
        StopObjectForces();
        objectRBInHand.AddForce(lastObjectVelocity, ForceMode.Force);
        lastObjectVelocity = Vector3.zero;

        // Reset player back to defaults
        playerStats.walkSpeed = defaultPlayerWalkSpeed;
        playerStats.SetCanJump(true);
        playerStats.SetCanRun(true);
        playerStats.canInteract = true;
        playerLook.ResetObjectTracking();

        // Reset object interaction to defaults
        isObjectGrabbed = false;
        objectInHand = null;
        objectRBInHand = null;
    }
}
