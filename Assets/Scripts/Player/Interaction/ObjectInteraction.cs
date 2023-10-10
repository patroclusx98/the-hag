using System;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    public Player player;
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
        defaultPlayerWalkSpeed = player.walkSpeed;
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
                    playerLook.SetObjectTracking(objectInHand.transform.position);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && player.CanInteract())
            {
                GrabObject();
            }
        }
    }

    /// <summary>
    /// Checks if the object is under the player or not
    /// </summary>
    /// <returns>True of the object is under the player</returns>
    private bool IsObjectUnderPlayer()
    {
        return player.gameObjectUnderPlayer == objectInHand;
    }

    /// <summary>
    /// Stops all forces that are affecting the object's rigidbody
    /// </summary>
    private void StopObjectForces()
    {
        objectRBInHand.velocity = Vector3.zero;
        objectRBInHand.angularVelocity = Vector3.zero;
        objectRBInHand.Sleep();
    }

    /// <summary>
    /// Sets the objects's movement velocity while it is being carried
    /// </summary>
    private void SetLastObjectVelocity()
    {
        float mouseXY = Math.Abs(Input.GetAxis("Mouse X")) + Math.Abs(Input.GetAxis("Mouse Y"));
        float forceMultiplier = Mathf.Clamp(objectRBInHand.mass, 1f, maxObjectCarryWeight);
        Vector3 throwDirection = Mathf.Clamp(mouseXY, 1f, 5f) * (objectInHand.transform.position - lastObjectPosition);

        lastObjectVelocity = player.strength * forceMultiplier * throwDirection;
        lastObjectPosition = objectInHand.transform.position;
    }

    /// <summary>
    /// Centers the object to the players hand while it is being carried
    /// </summary>
    private void CenterObjectToHand()
    {
        if (Vector3.Distance(objectInHand.transform.position, transform.position) > 0.1f)
        {
            /** The object is not yet centered to the player's hand **/

            float forceMultiplier = Mathf.Clamp(objectRBInHand.mass, 1f, maxObjectCarryWeight);

            objectRBInHand.drag = 40f;
            objectRBInHand.angularDrag = 40f;
            objectRBInHand.AddForce(objectRBInHand.drag * forceMultiplier * (transform.position - objectInHand.transform.position), ForceMode.Force);
        }
        else
        {
            /** The object is centered to the player's hand **/

            objectRBInHand.drag = defaultObjectDrag;
            objectRBInHand.angularDrag = defaultObjectAngularDrag;
            StopObjectForces();
            SetLastObjectVelocity();
        }
    }

    /// <summary>
    /// Carries the object in the center of the players hand
    /// </summary>
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

            lastObjectVelocity = player.strength * forceMultiplier * throwDirection;
            DropObject();
        }
    }

    /// <summary>
    /// Drags the object on the ground based on the player's movement
    /// </summary>
    private void DragObject()
    {
        /** Move object with player **/
        if (player.IsMoving())
        {
            Vector3 objectPosition = objectInHand.transform.position;
            Vector3 toPosition = objectPosition + player.horizontalVelocity * player.movementSpeed;

            objectInHand.transform.position = Vector3.Lerp(objectPosition, toPosition, Time.deltaTime);
        }

        /** Push object away **/
        if (Input.GetKey(KeyCode.Mouse1))
        {
            float forceMultiplier = Mathf.Clamp(objectRBInHand.mass, 1f, maxObjectDragWeight);
            Vector3 pushDirection = Quaternion.AngleAxis(90f, Vector3.up) * Vector3.Cross(playerLook.transform.forward, playerLook.transform.up);

            lastObjectVelocity = player.strength * forceMultiplier * pushDirection;
            DropObject();
        }
    }

    /// <summary>
    /// Attempts to grab the object the player is looking at
    /// </summary>
    private void GrabObject()
    {
        bool rayHit = Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out RaycastHit hitInfo, player.reachDistance, LayerMask.GetMask("Object"), QueryTriggerInteraction.Ignore);

        if (rayHit)
        {
            objectInHand = hitInfo.transform.gameObject;
            objectRBInHand = objectInHand.GetComponent<Rigidbody>();

            if (objectRBInHand != null)
            {
                if (objectRBInHand.mass <= maxObjectDragWeight && !IsObjectUnderPlayer())
                {
                    /** Get default values of the object **/
                    defaultObjectParent = objectInHand.transform.parent;
                    defaultObjectDrag = objectRBInHand.drag;
                    defaultObjectAngularDrag = objectRBInHand.angularDrag;

                    if (objectRBInHand.mass <= maxObjectCarryWeight)
                    {
                        /** Object will be carried **/

                        objectInHand.transform.parent = transform;
                        objectRBInHand.useGravity = false;
                    }
                    else
                    {
                        /** Object will be dragged **/

                        player.walkSpeed = player.walkSpeed * maxObjectCarryWeight / objectRBInHand.mass;
                        player.modifiers[Player.Modifier.DisableJump] = true;
                    }

                    player.modifiers[Player.Modifier.DisableRun] = true;
                    player.modifiers[Player.Modifier.Interacting] = Player.Interaction.Object;

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

    /// <summary>
    /// Runs when the objects's collider enters the players hand's trigger collider
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == objectInHand)
        {
            isObjectWithinReach = true;
        }
    }

    /// <summary>
    /// Runs when the objects's collider leaves the players hand's trigger collider
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == objectInHand)
        {
            isObjectWithinReach = false;
        }
    }

    /// <summary>
    /// Checks if the object should be dropped based on certain conditions
    /// </summary>
    /// <returns>True if the object should be dropped</returns>
    private bool ShouldDropObject()
    {
        bool rayHit1 = Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out RaycastHit hitInfo1, player.reachDistance, LayerMask.GetMask("Object"), QueryTriggerInteraction.Ignore);
        bool rayHit2 = Physics.Raycast(playerLook.transform.position, playerLook.transform.forward, out RaycastHit hitInfo2, player.reachDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore);

        /** Object is out of player's reach **/
        if ((!rayHit1 || hitInfo1.transform.gameObject != objectInHand) && !isObjectWithinReach)
        {
            return true;
        }

        /** Obstruction between hand and object **/
        if (rayHit2 && hitInfo2.transform.gameObject != objectInHand)
        {
            return true;
        }

        /** Object is under the player **/
        if (IsObjectUnderPlayer())
        {
            return true;
        }

        /** Player can no longer interact with the object **/
        if (!player.CanInteractWith(Player.Interaction.Object))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Drops the object the player is currently interacting with
    /// </summary>
    private void DropObject()
    {
        /** Reset object back to defaults **/
        objectInHand.transform.parent = defaultObjectParent;
        objectRBInHand.drag = defaultObjectDrag;
        objectRBInHand.angularDrag = defaultObjectAngularDrag;
        objectRBInHand.useGravity = true;

        /** Apply the last calculated velocity to the object **/
        StopObjectForces();
        objectRBInHand.AddForce(lastObjectVelocity, ForceMode.Force);
        lastObjectVelocity = Vector3.zero;

        /** Reset player stats back to defaults **/
        player.walkSpeed = defaultPlayerWalkSpeed;
        player.modifiers.Remove(Player.Modifier.DisableJump);
        player.modifiers.Remove(Player.Modifier.DisableRun);

        if (player.CanEndInteractionWith(Player.Interaction.Object))
        {
            player.modifiers.Remove(Player.Modifier.Interacting);
        }

        playerLook.ResetObjectTracking();

        /** Reset object interaction back to defaults **/
        isObjectGrabbed = false;
        objectInHand = null;
        objectRBInHand = null;
    }
}
