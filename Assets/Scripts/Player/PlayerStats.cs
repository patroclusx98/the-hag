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
    public float fallDamageSpeed = 1f;

    [Header("Player Stamina Attributes")]
    public bool isStaminaDrainEnabled = true;
    public float playerStamina = 100f;
    public float staminaChangeSpeed = 10f;

    [Header("Player Modifier Attributes")]
    public bool isAdrenalineOn = false;
    public float adrenalineModifier = 2f;
    public bool canRun = true;
    public bool canJump = true;
    [ReadOnlyInspector]
    public bool hasFallDamage = false;

    private bool canRunInternal = true;
    private bool canJumpInternal = true;
    private float fallDamageRecoveryTimer = 0f;
    private float fallDamageRecoveryLength = 0f;

    // Update is called once per frame
    private void Update()
    {
        float staminaDrainSpeed = staminaChangeSpeed;
        if (isAdrenalineOn)
        {
            staminaDrainSpeed = staminaChangeSpeed / adrenalineModifier;
        }

        HandleStamina(staminaDrainSpeed);

        if (hasFallDamage)
            HandleFallDamage();
    }

    private void HandleStamina(float staminaDrainSpeed)
    {
        if (playerStamina < 40f)
        {
            audioManager.PlayCollectionSound2D("Sound_Player_Breath", false, 0.5f);
        }

        if (isStaminaDrainEnabled)
        {
            if (playerMovement.isRunning && GetCanRun())
            {
                if (playerStamina > 0f)
                {
                    if (!playerMovement.hasJumped)
                    {
                        playerStamina -= Time.deltaTime * staminaDrainSpeed;
                    }
                }
                else
                {
                    playerStamina = 0f;
                    canRunInternal = false;
                }
            }
            if (playerMovement.hasJumped && GetCanJump())
            {
                if (playerStamina >= 10f)
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
        }

        if (playerStamina < 100f)
        {
            //Stamina recover
            if (!playerMovement.isRunning && !playerMovement.hasJumped)
            {
                playerStamina += Time.deltaTime * staminaChangeSpeed;
                if (playerStamina > 100f)
                {
                    playerStamina = 100f;
                }
            }

            //Run recover
            if (playerStamina >= 60f && !playerMovement.isRunning)
            {
                canRunInternal = true;
            }

            //Jump recover
            if (playerStamina >= staminaDrainSpeed && !playerMovement.hasJumped)
            {
                canJumpInternal = true;
            }
        }
    }

    private void HandleFallDamage()
    {
        if (fallDamageRecoveryTimer < fallDamageRecoveryLength)
        {
            fallDamageRecoveryTimer += Time.deltaTime;
        }
        else
        {
            fallDamageRecoveryTimer = 0f;
            hasFallDamage = false;
        }
    }

    public bool GetCanRun()
    {
        return canRun && canRunInternal;
    }

    public bool GetCanJump()
    {
        return canJump && canJumpInternal;
    }

    public void SetCanRun(bool canRun)
    {
        this.canRun = canRun;
    }

    public void SetCanJump(bool canJump)
    {
        this.canJump = canJump;
    }

    public void SetFallDamage(float lengthMultiplier)
    {
        fallDamageRecoveryLength = 1f * lengthMultiplier;

        fallDamageRecoveryTimer = 0f;
        hasFallDamage = true;
    }
}