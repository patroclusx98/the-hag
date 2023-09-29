using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public AudioManager audioManager;

    [Header("Player Interaction Attributes")]
    public bool canInteract = true;
    public float reachDistance = 1.1f;
    public float strength = 180f;

    [Header("Player Speed Attributes")]
    public float walkSpeed = 2f;
    public float sprintSpeed = 4f;
    public float climbSpeed = 1f;

    [Header("Player Stamina Attributes")]
    public bool isStaminaEnabled = true;
    public float playerStamina = 100f;
    public float staminaChangeSpeed = 10f;

    [Header("Player Modifier Attributes")]
    public bool isAdrenalineOn;
    public float adrenalineModifier = 2f;
    public bool canRun = true;
    public bool canJump = true;

    [Header("Player Modifier Inspector")]
    [ReadOnlyInspector]
    public bool hasFallDamage;
    [ReadOnlyInspector]
    public float fallDamageRecoveryTime;

    private bool canRunInternal = true;
    private bool canJumpInternal = true;

    // Update is called once per frame
    private void Update()
    {
        float staminaDrainSpeed = staminaChangeSpeed;

        if (isAdrenalineOn)
        {
            staminaDrainSpeed = staminaChangeSpeed / adrenalineModifier;
        }

        if (isStaminaEnabled)
        {
            HandleStamina(staminaDrainSpeed);
        }

        if (hasFallDamage)
        {
            HandleFallDamage();
        }
    }

    private void HandleStamina(float staminaDrainSpeed)
    {
        /** Player running stamina drain **/
        if (playerMovement.isRunning && GetCanRun())
        {
            if (playerStamina > 0f)
            {
                if (!playerMovement.isJumping)
                {
                    playerStamina -= staminaDrainSpeed * Time.deltaTime;
                }
            }
            else
            {
                playerStamina = 0f;
                canRunInternal = false;
            }
        }

        /** PLayer jumping stamina drain **/
        if (playerMovement.isJumping && GetCanJump())
        {
            if (playerStamina >= staminaDrainSpeed)
            {
                playerStamina -= staminaDrainSpeed;
                canJumpInternal = false;
            }
            else
            {
                playerStamina = 0f;
                canJumpInternal = false;
            }
        }

        if (playerStamina < 100f)
        {
            /** Recover player stamina **/
            if (!playerMovement.isRunning && !playerMovement.isJumping)
            {
                playerStamina += staminaChangeSpeed * Time.deltaTime;

                if (playerStamina > 100f)
                {
                    playerStamina = 100f;
                }
            }

            /** Recover running **/
            if (playerStamina >= 50f && !playerMovement.isRunning)
            {
                canRunInternal = true;
            }

            /** Recover jumping **/
            if (playerStamina >= staminaDrainSpeed && !playerMovement.isJumping)
            {
                canJumpInternal = true;
            }
        }

        /** Player tired sound play **/
        if (playerStamina < 40f)
        {
            audioManager.PlayCollectionSound2D("Sound_Player_Breath", false, 0.5f);
        }
    }

    private void HandleFallDamage()
    {
        if (fallDamageRecoveryTime > 0f)
        {
            fallDamageRecoveryTime -= Time.deltaTime;
        }
        else
        {
            fallDamageRecoveryTime = 0f;
            hasFallDamage = false;
        }
    }

    public bool GetCanRun()
    {
        return canRun && canRunInternal && !hasFallDamage;
    }

    public bool GetCanJump()
    {
        return canJump && canJumpInternal && !hasFallDamage;
    }

    public void SetCanRun(bool canRun)
    {
        this.canRun = canRun;
    }

    public void SetCanJump(bool canJump)
    {
        this.canJump = canJump;
    }

    public void SetFallDamage(float fallDamageRecoveryTime)
    {
        this.fallDamageRecoveryTime += fallDamageRecoveryTime;
        hasFallDamage = true;
    }
}