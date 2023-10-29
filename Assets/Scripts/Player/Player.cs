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
    public enum Interaction
    {
        Object, Item, Window, Door, Inventory
    }

    public CharacterController characterController;
    public PlayerAnimator playerAnimator;
    public PlayerLook playerLook;
    public PlayerInventory playerInventory;
    public AudioManager audioManager;

    [Header("Player Movement Attributes")]
    public LayerMask groundMask;
    public float jumpHeight = 1f;
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

        if (!isMoving || (!CanMove() && !CanClimb()))
        {
            ResetMovement();
        }

        if (Input.GetKeyDown(KeyCode.Space) && CanJump())
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && CanToggleCrouch())
        {
            ToggleCrouch();
        }

        if (!isClimbing)
        {
            ApplyGravity();
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

    /// <summary>
    /// Returns if the player is able to horizontally move
    /// </summary>
    /// <returns>True if able to move/returns>
    public bool CanMove()
    {
        return !isClimbing &&
            !modifiers.ContainsKey(Modifier.DisableMovement);
    }

    /// <summary>
    /// Returns if the player is able to walk
    /// </summary>
    /// <returns>True if able to walk</returns>
    public bool CanWalk()
    {
        return CanMove() &&
            !modifiers.ContainsKey(Modifier.WallBlock);
    }

    /// <summary>
    /// Returns if the player is able to run
    /// </summary>
    /// <returns>True if able to run</returns>
    public bool CanRun()
    {
        return CanMove() && !isCrouching &&
            !modifiers.ContainsKey(Modifier.DisableRun) &&
            !modifiers.ContainsKey(Modifier.WallBlock) &&
            !modifiers.ContainsKey(Modifier.OutOfStamina) &&
            !modifiers.ContainsKey(Modifier.FallDamage);
    }

    /// <summary>
    /// Returns if the player is moving
    /// </summary>
    /// <returns>True if moving</returns>
    public bool IsMoving()
    {
        return isWalking || isRunning;
    }

    /// <summary>
    /// Moves the player based on user inputs and handles collisions with walls that block movement
    /// </summary>
    private void Move()
    {
        float walkX = Input.GetAxis("Horizontal");
        float walkY = Input.GetAxis("Vertical");

        /** Build up horizontal velocity based on inputs **/
        if (isGrounded)
        {
            horizontalVelocity = transform.forward * walkY + transform.right * walkX;

            /** Strafe running speed modulation **/
            if (walkY != 0f && walkX != 0f)
            {
                horizontalVelocity *= 0.71f;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && walkY > 0f && CanRun())
        {
            /** Player is running **/

            playerAnimator.SetIsRunning(true);
            movementSpeed = sprintSpeed;

            /** Handle stamina decrease **/
            float modifier = modifiers.TryGetValue(Modifier.AdrenalineBoost, out dynamic adrenalineModifier) ? adrenalineModifier : 1f;
            stamina -= staminaChangeSpeed / modifier * Time.deltaTime;
        }
        else if (CanWalk())
        {
            /** Player is walking **/

            playerAnimator.SetIsWalking(true);
            movementSpeed = walkSpeed;

            if (isCrouching || modifiers.ContainsKey(Modifier.FallDamage))
            {
                movementSpeed *= 0.5f;
            }
        }

        /** Check if movement direction is blocked by wall **/
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

                playerAnimator.SetIsWalking(false);
                playerAnimator.SetIsRunning(false);
                movementSpeed = 0f;
            }
        }

        characterController.Move(movementSpeed * Time.deltaTime * horizontalVelocity);
    }

    /// <summary>
    /// Resets the player's movement parameters
    /// </summary>
    private void ResetMovement()
    {
        playerAnimator.SetIsWalking(false);
        playerAnimator.SetIsRunning(false);
        movementSpeed = 0f;
        horizontalVelocity = Vector3.zero;
    }

    /** PLAYER CLIMBING METHODS **/

    /// <summary>
    /// Returns if the player is able to climb
    /// </summary>
    /// <returns>True if able to climb</returns>
    public bool CanClimb()
    {
        return isClimbing && !modifiers.ContainsKey(Modifier.DisableMovement);
    }

    /// <summary>
    /// Moves the player in a vertical direction based on user inputs
    /// </summary>
    private void Climb()
    {
        float walkX = Input.GetAxis("Horizontal");
        float walkY = Input.GetAxis("Vertical");

        horizontalVelocity = transform.up * walkY + transform.right * walkX;

        /** Allow forwards movement off the ground or backwards movement on the ground **/
        if ((!isGrounded && walkY > 0f) || (isGrounded && walkY < 0f))
        {
            horizontalVelocity += transform.forward * walkY;
        }

        movementSpeed = climbSpeed;

        if (isCrouching || modifiers.ContainsKey(Modifier.FallDamage))
        {
            movementSpeed *= 0.5f;
        }

        characterController.Move(movementSpeed * Time.deltaTime * horizontalVelocity);
    }

    /** PLAYER JUMPING METHODS **/

    /// <summary>
    /// Returns if the player is able to jump
    /// </summary>
    /// <returns>True if able to jump</returns>
    public bool CanJump()
    {
        float playerRadius = characterController.radius * transform.localScale.y;
        float playerJumpHeight = (characterController.height + jumpHeight) * transform.localScale.y - playerRadius;

        return isGrounded && !isClimbing && !isJumping && (!isCrouching || hasFullyCrouched) &&
            !Physics.SphereCast(transform.position, playerRadius * 0.85f, transform.up, out _, playerJumpHeight * 0.9f, groundMask, QueryTriggerInteraction.Ignore) &&
            !modifiers.ContainsKey(Modifier.DisableMovement) &&
            !modifiers.ContainsKey(Modifier.DisableJump) &&
            !modifiers.ContainsKey(Modifier.OutOfStamina) &&
            !modifiers.ContainsKey(Modifier.FallDamage);
    }

    /// <summary>
    /// Sets the player's jump animation state and applies a positive vertical velocity to the player
    /// </summary>
    private void Jump()
    {
        playerAnimator.SetJumping();
        verticalVelocity.y = Mathf.Sqrt(gravityForce) * jumpHeight;

        PlayJumpingSound();
    }

    /** PLAYER CROUCHING METHODS **/

    /// <summary>
    /// Returns if the player is able to crouch or stand up
    /// </summary>
    /// <returns>True if able to crouch or stand up</returns>
    public bool CanToggleCrouch()
    {
        float playerRadius = characterController.radius * transform.localScale.y;
        float playerStandHeight = characterController.height * transform.localScale.y - playerRadius;

        return (isGrounded || isClimbing) && !isJumping &&
            (!hasFullyCrouched || !Physics.SphereCast(transform.position, playerRadius * 0.85f, transform.up, out _, playerStandHeight, groundMask, QueryTriggerInteraction.Ignore)) &&
            !modifiers.ContainsKey(Modifier.DisableMovement);
    }

    /// <summary>
    /// Toggles the player's crouch animation state
    /// </summary>
    private void ToggleCrouch()
    {
        if (!isCrouching)
        {
            playerAnimator.SetCrouching();

            PlayCrouchingSound();
        }
        else if (hasFullyCrouched)
        {
            playerAnimator.ResetFullyCrouched();

            PlayCrouchingSound();
        }
    }

    /** PLAYER STAMINA METHODS **/

    /// <summary>
    /// Returns if the player is able to recover stamina
    /// </summary>
    /// <returns>True if able to recover stamina</returns>
    public bool CanRecoverStamina()
    {
        return !modifiers.ContainsKey(Modifier.DisableStamina);
    }

    /// <summary>
    /// Recovers the players stamina level and handles the out of stamina modifier
    /// </summary>
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

    /// <summary>
    /// Returns if the player is able to recover fall damage
    /// </summary>
    /// <returns>True if able to recover fall damage</returns>
    public bool CanRecoverFallDamage()
    {
        return modifiers.ContainsKey(Modifier.FallDamage);
    }

    /// <summary>
    /// Recovers the player from the fall damage modifier
    /// </summary>
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

    /// <summary>
    /// Applies the vertical velocity to the player and handles ground impacts
    /// </summary>
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
                }

                if (verticalVelocity.y < impactTolerance)
                {
                    // TODO Add impact sounds
                }

                playerAnimator.SetImpactVelocity(verticalVelocity.y);
                verticalVelocity.y = -Mathf.Sqrt(gravityForce);
            }
        }

        characterController.Move(verticalVelocity * Time.deltaTime);
    }

    /** PLAYER COLLISION METHODS **/

    /// <summary>
    /// Runs when the player character controller's collider hits another collider
    /// </summary>
    /// <param name="hit">The character controller collider's hit details</param>
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.moveDirection.y == 0f)
        {
            /** Horizontal only collisions **/

            CheckForWallHit(hit);
        }
        else
        {
            /** Every other collision **/

            SetGameObjectUnderPlayer(hit);
        }
    }

    /// <summary>
    /// Checks if the player walked into a wall and applies the wall block modifier to prevent further movement towards that direction
    /// </summary>
    /// <param name="hit">The character controller collider's hit details</param>
    private void CheckForWallHit(ControllerColliderHit hit)
    {
        /** Compare surface normal to direction of movement when colliding **/
        if (Vector3.Dot(hit.moveDirection, hit.normal) < -wallHitTolerance)
        {
            modifiers.TryAdd(Modifier.WallBlock, hit.moveDirection);
        }
    }

    /// <summary>
    /// Sets the game object that is under the player
    /// </summary>
    /// <param name="hit">The character controller collider's hit details</param>
    private void SetGameObjectUnderPlayer(ControllerColliderHit hit)
    {
        Vector3 pointOfHitLocal = transform.InverseTransformPoint(hit.point);

        if (pointOfHitLocal.y < (characterController.height * 0.5f) - 0.3f)
        {
            gameObjectUnderPlayer = hit.gameObject;
        }
    }

    /** PLAYER INTERACTION METHODS **/

    /// <summary>
    /// Returns if the player is able to interact with anything
    /// </summary>
    /// <returns>True if able to interact, False if already interacting with something</returns>
    public bool CanInteract()
    {
        return !modifiers.ContainsKey(Modifier.DisableInteraction) &&
            !modifiers.ContainsKey(Modifier.Interacting);
    }

    /// <summary>
    /// Returns if the player is able to preform the specified interaction
    /// </summary>
    /// <param name="interaction">The interaction to check</param>
    /// <returns>True if able to interact, False if already interacting with something else</returns>
    public bool CanInteractWith(Interaction interaction)
    {
        return !modifiers.ContainsKey(Modifier.DisableInteraction) &&
            (!modifiers.ContainsKey(Modifier.Interacting) || modifiers[Modifier.Interacting] == interaction);
    }

    /// <summary>
    /// Returns if the player is able to end the specified interaction
    /// </summary>
    /// <param name="interaction">The interaction to check</param>
    /// <returns>True if able to end the interaction, False if interacting with something else</returns>
    public bool CanEndInteractionWith(Interaction interaction)
    {
        return modifiers.ContainsKey(Modifier.DisableInteraction) ||
            (modifiers.ContainsKey(Modifier.Interacting) && modifiers[Modifier.Interacting] == interaction);
    }

    /** PLAYER SOUND METHODS **/

    private void PlayJumpingSound()
    {
        audioManager.PlayCollectionSound("player_jump", true);
    }

    private void PlayCrouchingSound()
    {
        audioManager.PlayCollectionSound("player_crouch", true);
    }

    private void PlayerBreathSound()
    {
        audioManager.PlayCollectionSound("player_breath", false, 0.2f);
    }
}
