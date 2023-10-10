using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum Modifier
    {
        DisableStamina, AdrenalineBoost,
        DisableInteraction, Interacting,
        DisableMovement, DisableRun, DisableJump, OutOfStamina, FallDamage, WallBlock
    }
    public enum Interaction { Object, Item, Window, Door, Inventory }

    public CharacterController characterController;
    public PlayerAnimator playerAnimator;
    public PlayerLook playerLook;
    public AudioManager audioManager;

    [Header("Player Movement Attributes")]
    public LayerMask groundMask;
    public float jumpHeight = 1f;
    public float maxObjectPushWeight = 5f;
    public float wallHitTolerance = 0.8f;
    public float gravityForce = 25f;
    public float impactTolerance = -7.5f;
    public float fallDamageTolerance = -15f;

    [Header("Player Interaction Attributes")]
    public float reachDistance = 1.1f;
    public float strength = 180f;

    [Header("Player Speed Attributes")]
    public float walkSpeed = 2f;
    public float sprintSpeed = 4f;
    public float climbSpeed = 1f;

    [Header("Player Stamina Attributes")]
    public float stamina = 100f;
    public float staminaChangeSpeed = 10f;

    [Header("Player Movement Inspector")]
    [ReadOnlyInspector]
    public float movementSpeed;
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

    public Dictionary<Modifier, dynamic> modifiers = new Dictionary<Modifier, dynamic>();

    // Update is called once per frame
    private void Update()
    {
        playerAnimator.SetIsGrounded(characterController.isGrounded);

        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);

        if (isMoving && CanMove())
        {
            Move();
        }

        if (isMoving && CanClimb())
        {
            Climb();
        }

        if (Input.GetKeyDown(KeyCode.Space) && CanJump())
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && CanCrouch())
        {
            ToggleCrouch();
        }

        if (!isClimbing)
        {
            ApplyGravity();
        }

        if (!isMoving || (!CanMove() && !CanClimb()))
        {
            isWalking = false;
            isRunning = false;
            movementSpeed = 0f;
            horizontalVelocity = Vector3.zero;
        }

        if (CanRecoverStamina())
        {
            RecoverStamina();
        }

        if (CanRecoverFallDamage())
        {
            RecoverFallDamage();
        }
    }

    /** PLAYER MOVEMENT METHODS **/

    public bool CanMove()
    {
        return !isClimbing && !modifiers.ContainsKey(Modifier.DisableMovement);
    }

    public bool CanWalk()
    {
        return CanMove() && !modifiers.ContainsKey(Modifier.WallBlock);
    }

    public bool CanRun()
    {
        return CanMove() && !isCrouching &&
            !modifiers.ContainsKey(Modifier.DisableRun) &&
            !modifiers.ContainsKey(Modifier.OutOfStamina) &&
            !modifiers.ContainsKey(Modifier.FallDamage) &&
            !modifiers.ContainsKey(Modifier.WallBlock);
    }

    public bool IsMoving()
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

        if (Input.GetKey(KeyCode.LeftShift) && walkY > 0f && CanRun())
        {
            /** Player is running **/

            isWalking = false;
            isRunning = true;
            movementSpeed = sprintSpeed;

            if (!isJumping)
            {
                if (modifiers.TryGetValue(Modifier.AdrenalineBoost, out dynamic adrenalineModifier))
                {
                    stamina -= staminaChangeSpeed / adrenalineModifier * Time.deltaTime;
                }
                else
                {
                    stamina -= staminaChangeSpeed * Time.deltaTime;
                }
            }

            PlayRunSound();
        }
        else if (CanWalk())
        {
            /** Player is walking **/

            isWalking = true;
            isRunning = false;
            movementSpeed = walkSpeed;

            if (isCrouching || modifiers.ContainsKey(Modifier.FallDamage))
            {
                movementSpeed *= 0.5f;
            }

            PlayWalkSound();
        }

        if (modifiers.TryGetValue(Modifier.WallBlock, out dynamic directionOfWallCollision))
        {
            if (isJumping || isCrouching || Vector3.Dot(directionOfWallCollision, horizontalVelocity) < wallHitTolerance)
            {
                /** Player no longer blocked by wall **/

                modifiers.Remove(Modifier.WallBlock);
            }
            else
            {
                /** Player blocked by wall **/

                isWalking = false;
                isRunning = false;
                movementSpeed = 0f;
                horizontalVelocity = Vector3.zero;
            }
        }

        characterController.Move(movementSpeed * Time.deltaTime * horizontalVelocity);
    }

    /** PLAYER CLIMBING METHODS **/

    public bool CanClimb()
    {
        return isClimbing && !modifiers.ContainsKey(Modifier.DisableMovement);
    }

    private void Climb()
    {
        float walkX = Input.GetAxis("Horizontal");
        float walkY = Input.GetAxis("Vertical");

        horizontalVelocity = gameObject.transform.up * walkY + gameObject.transform.right * walkX;

        if ((!isGrounded && walkY > 0f) || (isGrounded && walkY < 0f))
        {
            horizontalVelocity += gameObject.transform.forward * walkY;
        }

        isWalking = false;
        isRunning = false;
        movementSpeed = climbSpeed;

        if (isCrouching || modifiers.ContainsKey(Modifier.FallDamage))
        {
            movementSpeed *= 0.5f;
        }

        characterController.Move(movementSpeed * Time.deltaTime * horizontalVelocity);
    }

    /** PLAYER JUMPING METHODS **/

    public bool CanJump()
    {
        return CanMove() && isGrounded && !isJumping && (!isCrouching || hasFullyCrouched) &&
            !modifiers.ContainsKey(Modifier.DisableJump) &&
            !modifiers.ContainsKey(Modifier.OutOfStamina) &&
            !modifiers.ContainsKey(Modifier.FallDamage);
    }

    private void Jump()
    {
        float playerRadius = characterController.radius * transform.localScale.y;
        float playerJumpHeight = (characterController.height + jumpHeight) * transform.localScale.y - playerRadius;

        if (!Physics.SphereCast(gameObject.transform.position, playerRadius * 0.85f, transform.up, out _, playerJumpHeight * 0.9f, groundMask, QueryTriggerInteraction.Ignore))
        {
            playerAnimator.SetJumping();
            verticalVelocity.y = Mathf.Sqrt(gravityForce) * jumpHeight;

            if (modifiers.TryGetValue(Modifier.AdrenalineBoost, out dynamic adrenalineModifier))
            {
                stamina -= staminaChangeSpeed / adrenalineModifier;
            }
            else
            {
                stamina -= staminaChangeSpeed;
            }

            PlayJumpingSound();
        }
    }

    /** PLAYER CROUCHING METHODS **/

    public bool CanCrouch()
    {
        return (isGrounded || isClimbing) && !isJumping &&
            !modifiers.ContainsKey(Modifier.DisableMovement);
    }

    private void ToggleCrouch()
    {
        if (!isCrouching)
        {
            playerAnimator.SetCrouching();

            PlayCrouchingSound();
        }
        else if (hasFullyCrouched)
        {
            float playerRadius = characterController.radius * transform.localScale.y;
            float playerStandHeight = characterController.height * transform.localScale.y - playerRadius;

            if (!Physics.SphereCast(gameObject.transform.position, playerRadius * 0.85f, transform.up, out _, playerStandHeight, groundMask, QueryTriggerInteraction.Ignore))
            {
                playerAnimator.ResetFullyCrouched();

                PlayCrouchingSound();
            }
        }
    }

    /** PLAYER STAMINA METHODS **/

    public bool CanRecoverStamina()
    {
        return !modifiers.ContainsKey(Modifier.DisableStamina);
    }

    private void RecoverStamina()
    {
        /** Recover player stamina **/
        if (stamina < 100f && !isRunning && !isJumping)
        {
            if (modifiers.TryGetValue(Modifier.AdrenalineBoost, out dynamic adrenalineModifier))
            {
                stamina += staminaChangeSpeed * adrenalineModifier * Time.deltaTime;
            }
            else
            {
                stamina += staminaChangeSpeed * Time.deltaTime;
            }

            if (stamina > 100f)
            {
                stamina = 100f;
            }
        }

        /** Recover out of stamina **/
        if (modifiers.ContainsKey(Modifier.OutOfStamina))
        {
            if (stamina >= 40f)
            {
                modifiers.Remove(Modifier.OutOfStamina);
            }

            PlayerBreathSound();
        }

        /** Apply out of stamina **/
        if (stamina <= 0f)
        {
            modifiers[Modifier.OutOfStamina] = null;
            stamina = 0f;
        }
    }

    /** PLAYER FALL DAMAGE METHODS **/

    public bool CanRecoverFallDamage()
    {
        return modifiers.ContainsKey(Modifier.FallDamage);
    }

    private void RecoverFallDamage()
    {
        if (modifiers[Modifier.FallDamage] > 0f)
        {
            modifiers[Modifier.FallDamage] -= Time.deltaTime;
        }
        else
        {
            modifiers.Remove(Modifier.FallDamage);
        }
    }

    /** PLAYER GRAVITY METHODS **/

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            /** Player is airborne **/

            verticalVelocity.y -= gravityForce * Time.deltaTime;
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

                    modifiers[Modifier.FallDamage] = Mathf.Clamp(fallDamageRecoveryTime, 5f, 15f);

                    PlayFallDamageSound();
                }
                else if (verticalVelocity.y < impactTolerance)
                {
                    PlayImpactSound();
                }

                playerAnimator.SetImpactVelocity(verticalVelocity.y);
                verticalVelocity.y = -Mathf.Sqrt(gravityForce);
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

            SetGameObjectUnderPlayer(hit);
        }
    }

    //Pushes small rigid bodies around when collided with
    //Returns true if object can be pushed and/or is being pushed
    private bool PushRigidBodyObjects(ControllerColliderHit hit)
    {
        Rigidbody objectRB = hit.rigidbody;

        /** Not a rigid body **/
        if (objectRB == null || objectRB.isKinematic)
        {
            return false;
        }

        /** Do not push heavy objects **/
        if (objectRB.mass > maxObjectPushWeight)
        {
            return false;
        }

        /** Calculate push direction from move direction
            Only push objects along X and Z **/
        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);

        // Apply the push
        objectRB.velocity = Mathf.Clamp(1f / objectRB.mass, 0.5f, 1.5f) * pushDirection;

        return true;
    }

    private void CheckForWallHit(ControllerColliderHit hit)
    {
        /** Ignore objects **/
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Object"))
        {
            return;
        }

        /** Compare surface normal to direction of movement when colliding **/
        if (Vector3.Dot(hit.moveDirection, hit.normal) < -wallHitTolerance)
        {
            modifiers.TryAdd(Modifier.WallBlock, hit.moveDirection);
        }
    }

    private void SetGameObjectUnderPlayer(ControllerColliderHit hit)
    {
        Vector3 pointOfHitLocal = transform.InverseTransformPoint(hit.point);

        if (pointOfHitLocal.y < (characterController.height * 0.5f) - 0.3f)
        {
            gameObjectUnderPlayer = hit.collider.gameObject;
        }
    }

    /** PLAYER INTERACTION METHODS **/

    public bool CanInteract()
    {
        return !modifiers.ContainsKey(Modifier.DisableInteraction) &&
            !modifiers.ContainsKey(Modifier.Interacting);
    }

    public bool CanInteractWith(Interaction interaction)
    {
        return !modifiers.ContainsKey(Modifier.DisableInteraction) &&
            (!modifiers.ContainsKey(Modifier.Interacting) || modifiers[Modifier.Interacting] == interaction);
    }

    public bool CanEndInteractionWith(Interaction interaction)
    {
        return modifiers.ContainsKey(Modifier.DisableInteraction) ||
            (modifiers.ContainsKey(Modifier.Interacting) && modifiers[Modifier.Interacting] == interaction);
    }

    /** PLAYER SOUND METHODS **/

    private void PlayWalkSound()
    {
        if (isGrounded && !isCrouching && horizontalVelocity.magnitude > 0.35f)
        {
            float baseSpeed = 1f;
            float stepSpeed = Mathf.Clamp(baseSpeed / movementSpeed, 0.4f, 0.75f);

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
        audioManager.PlayCollectionSound("Sound_Player_Jump", true);
    }

    private void PlayCrouchingSound()
    {
        audioManager.PlayCollectionSound("Sound_Player_Crouch", true);
    }

    private void PlayerBreathSound()
    {
        audioManager.PlayCollectionSound("Sound_Player_Breath", false, 0.2f);
    }

    private void PlayImpactSound()
    {
        audioManager.PlayCollectionSound("Sound_Step_Run_Dirt");
    }

    private void PlayFallDamageSound()
    {
        audioManager.PlayCollectionSound("Sound_Step_Run_Dirt");
    }
}
