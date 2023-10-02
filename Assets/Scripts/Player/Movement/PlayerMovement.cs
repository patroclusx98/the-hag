using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerStats playerStats;
    public PlayerLook playerLook;
    public CharacterController characterController;
    public Animator playerAnimator;
    public AudioManager audioManager;

    [Header("Player Movement Attributes")]
    public LayerMask groundMask;
    public float jumpHeight = 1f;
    public float wallHitTolerance = 0.8f;
    public float maxObjectPushWeight = 5f;

    [Header("Player Gravity Attributes")]
    public float gravityForce = -25f;
    public float groundedGravityForce = -4f;
    public float impactTolerance = -7.5f;
    public float fallDamageTolerance = -15f;

    [Header("Player Movement Inspector")]
    [ReadOnlyInspector]
    public float playerSpeed;
    [ReadOnlyInspector]
    public Vector3 horizontalVelocity;
    [ReadOnlyInspector]
    public Vector3 verticalVelocity;
    [ReadOnlyInspector]
    public bool isGrounded;
    [ReadOnlyInspector]
    public bool isWalking;
    [ReadOnlyInspector]
    public bool isRunning;
    [ReadOnlyInspector]
    public bool isClimbing;
    [ReadOnlyInspector]
    public bool isJumping;
    [ReadOnlyInspector]
    public bool isCrouching;
    [ReadOnlyInspector]
    public bool hasFullyCrouched;
    [ReadOnlyInspector]
    public GameObject gameObjectUnderPlayer;

    private Vector3 directionOfWallCollision;
    private bool didWalkIntoWall;

    // Update is called once per frame
    private void Update()
    {
        isGrounded = characterController.isGrounded;
        playerAnimator.SetBool("IsGrounded", isGrounded);

        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        /** Player moving **/
        if (isMoving && !isClimbing)
        {
            Move();
        }

        /** Player climbing **/
        if (isMoving && isClimbing)
        {
            Climb();
        }

        /** Player jumping **/
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        /** Player crouching **/
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ToggleCrouch();
        }

        /** Player gravity **/
        if (!isClimbing)
        {
            ApplyGravity();
        }

        /** Player idle **/
        if (!isMoving)
        {
            horizontalVelocity.Set(0f, 0f, 0f);
            playerSpeed = 0f;
            isWalking = false;
            isRunning = false;
        }
    }

    /** PLAYER MOVEMENT METHODS **/

    public bool IsPlayerMoving()
    {
        return isWalking || isRunning;
    }

    private void Move()
    {
        float walkX = Input.GetAxis("Horizontal");
        float walkY = Input.GetAxis("Vertical");

        if (isGrounded)
        {
            horizontalVelocity = gameObject.transform.forward * walkY + gameObject.transform.right * walkX;

            /** Strafe running speed modulation **/
            if (walkY != 0f && walkX != 0f)
            {
                horizontalVelocity *= 0.71f;
            }
        }

        if (!didWalkIntoWall)
        {
            if (Input.GetKey(KeyCode.LeftShift) && walkY > 0f && !isCrouching && playerStats.GetCanRun())
            {
                /** Player running **/

                playerSpeed = playerStats.sprintSpeed;
                isRunning = true;
                isWalking = false;
                PlayRunSound();
            }
            else
            {
                /** Player walking **/

                if (isCrouching && playerStats.hasFallDamage)
                {
                    playerSpeed = playerStats.walkSpeed * 0.25f;
                }
                else if (isCrouching || playerStats.hasFallDamage)
                {
                    playerSpeed = playerStats.walkSpeed * 0.5f;
                }
                else
                {
                    playerSpeed = playerStats.walkSpeed;
                }

                isWalking = true;
                isRunning = false;
                PlayWalkSound();
            }

            characterController.Move(playerSpeed * Time.deltaTime * horizontalVelocity);
        }
        else
        {
            if (isJumping || isCrouching || Vector3.Dot(directionOfWallCollision, horizontalVelocity) < wallHitTolerance)
            {
                /** Player no longer hits wall **/

                didWalkIntoWall = false;
            }
            else
            {
                /** Player still hits wall **/

                playerSpeed = 0f;
                isRunning = false;
                isWalking = false;
            }
        }
    }

    private void Climb()
    {
        float walkX = Input.GetAxis("Horizontal");
        float walkY = Input.GetAxis("Vertical");

        horizontalVelocity = gameObject.transform.up * walkY + gameObject.transform.right * walkX;

        if (playerStats.hasFallDamage)
        {
            playerSpeed = playerStats.climbSpeed * 0.5f;
        }
        else
        {
            playerSpeed = playerStats.climbSpeed;
        }

        if ((!isGrounded && walkY > 0f) || (isGrounded && walkY < 0f))
        {
            horizontalVelocity += gameObject.transform.forward * walkY;
        }

        characterController.Move(playerSpeed * Time.deltaTime * horizontalVelocity);
    }

    /** PLAYER JUMPING METHODS **/

    private void Jump()
    {
        if (isGrounded && !isJumping && (!isCrouching || hasFullyCrouched) && !isClimbing && playerStats.GetCanJump())
        {
            float playerRadius = characterController.radius * transform.localScale.y;
            float playerJumpHeight = (characterController.height + jumpHeight) * transform.localScale.y - playerRadius;

            if (!Physics.SphereCast(gameObject.transform.position, playerRadius * 0.85f, transform.up, out _, playerJumpHeight * 0.9f, groundMask, QueryTriggerInteraction.Ignore))
            {
                isJumping = true;
                playerAnimator.SetBool("IsJumping", isJumping);
                PlayJumpingSound();
            }
        }
    }

    /** PLAYER CROUCHING METHODS **/

    private void ToggleCrouch()
    {
        if ((isGrounded || isClimbing) && !isJumping)
        {
            if (!isCrouching)
            {
                isCrouching = true;
                playerAnimator.SetBool("IsCrouching", isCrouching);
                PlayCrouchingSound();
            }
            else if (hasFullyCrouched)
            {
                float playerRadius = characterController.radius * transform.localScale.y;
                float playerStandHeight = characterController.height * transform.localScale.y - playerRadius;

                if (!Physics.SphereCast(gameObject.transform.position, playerRadius * 0.85f, transform.up, out _, playerStandHeight, groundMask, QueryTriggerInteraction.Ignore))
                {
                    hasFullyCrouched = false;
                    playerAnimator.SetBool("HasFullyCrouched", hasFullyCrouched);
                    PlayCrouchingSound();
                }
            }
        }
    }

    /** PLAYER GRAVITY METHODS **/

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            /** Player is airborne **/

            verticalVelocity.y += gravityForce * Time.deltaTime;
            playerAnimator.SetFloat("VerticalVelocityY", verticalVelocity.y);
        }
        else
        {
            /** Player is on the ground **/

            if (verticalVelocity.y <= 0f)
            {
                if (verticalVelocity.y < fallDamageTolerance)
                {
                    float multiplier = Mathf.Pow(5f, 1f / 3f) / fallDamageTolerance;
                    float fallDamageRecoveryTime = Mathf.Abs(Mathf.Pow(verticalVelocity.y * multiplier, 3f));

                    playerStats.SetFallDamage(Mathf.Clamp(fallDamageRecoveryTime, 5f, 15f));
                    playerAnimator.SetFloat("VerticalVelocityY", verticalVelocity.y);
                    PlayFallDamageSound();
                }
                else if (verticalVelocity.y < impactTolerance)
                {
                    playerAnimator.SetFloat("VerticalVelocityY", verticalVelocity.y);
                    PlayImpactSound();
                }

                verticalVelocity.y = groundedGravityForce;
            }
        }

        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    /** PLAYER COLLISION METHODS **/

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.moveDirection.y == 0f)
        {
            /** Horizontal only collisions **/

            if (!PushRigidBodyObjects(hit))
            {
                CheckForWallHit(hit);
            }
        }
        else
        {
            /** Every other collision **/

            CheckObjectBelowPlayer(hit);
        }
    }

    //Pushes small rigid bodies around when collided with
    //Returns true if object can be pushed and/or is being pushed
    private bool PushRigidBodyObjects(ControllerColliderHit hit)
    {
        Rigidbody objectRB = hit.rigidbody;

        // Not a rigid body
        if (objectRB == null || objectRB.isKinematic)
        {
            return false;
        }

        // Do not push heavy objects
        if (objectRB.mass > maxObjectPushWeight)
        {
            return false;
        }

        // Calculate push direction from move direction
        // Only push objects along X and Z
        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);

        // Apply the push
        objectRB.velocity = Mathf.Clamp(1f / objectRB.mass, 0.5f, 1.5f) * pushDirection;

        return true;
    }

    //Returns true if player is walking into a wall
    private void CheckForWallHit(ControllerColliderHit hit)
    {
        // Ignore objects
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Object"))
        {
            return;
        }

        //Compare surface normal to direction of movement when colliding
        if (Vector3.Dot(hit.moveDirection, hit.normal) < -wallHitTolerance)
        {
            directionOfWallCollision = hit.moveDirection;
            didWalkIntoWall = true;
        }
    }

    //Check and sets the game object that is under the player
    private void CheckObjectBelowPlayer(ControllerColliderHit hit)
    {
        Vector3 pointOfHitLocal = transform.InverseTransformPoint(hit.point);

        if (pointOfHitLocal.y < (characterController.height * 0.5f) - 0.3f)
        {
            gameObjectUnderPlayer = hit.collider.gameObject;
        }
    }

    /** PLAYER SOUND METHODS **/

    private void PlayWalkSound()
    {
        if (isGrounded && !isCrouching && horizontalVelocity.magnitude > 0.35f)
        {
            float baseSpeed = 1f;
            float stepSpeed = Mathf.Clamp(baseSpeed / playerSpeed, 0.4f, 0.75f);

            audioManager.PlayCollectionSound("Sound_Step_Walk_Dirt", true, stepSpeed);
        }
    }

    private void PlayRunSound()
    {
        if (isGrounded && horizontalVelocity.magnitude > 0.35f)
        {
            audioManager.PlayCollectionSound("Sound_Step_Run_Dirt", true, 0.3f);
        }

    }

    private void PlayJumpingSound()
    {
        audioManager.PlayCollectionSound("Sound_Player_Jump", true, 0f);
    }

    private void PlayCrouchingSound()
    {
        audioManager.PlayCollectionSound("Sound_Player_Crouch", true, 0f);
    }

    private void PlayImpactSound()
    {
        audioManager.PlayCollectionSound("Sound_Step_Run_Dirt", false, 0f);
    }

    private void PlayFallDamageSound()
    {
        audioManager.PlayCollectionSound("Sound_Step_Run_Dirt", false, 0f);
    }
}
