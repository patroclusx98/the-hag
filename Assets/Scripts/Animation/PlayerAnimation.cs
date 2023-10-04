using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator playerAnimator;
    public PlayerMovement playerMovement;

    /// <summary>
    /// Adds upwards vertical velocity to the player
    /// <para>This is used by the player animator</para>
    /// </summary>
    public void SetJump()
    {
        playerMovement.verticalVelocity.y = Mathf.Sqrt(-playerMovement.jumpHeight * playerMovement.gravityForce);
    }

    /// <summary>
    /// Resets the jump parameters for the player and player animator
    /// <para>This is used by the player animator</para>
    /// </summary>
    public void ResetJump()
    {
        playerMovement.isJumping = false;
        playerAnimator.SetBool("IsJumping", false);
    }

    /// <summary>
    /// Sets the crouch parameters for the player and player animator
    /// <para>This is used by the player animator</para>
    /// </summary>
    public void SetCrouch()
    {
        playerMovement.hasFullyCrouched = true;
        playerAnimator.SetBool("HasFullyCrouched", true);
    }

    /// <summary>
    /// Resets the crouch parameters for the player and player animator
    /// <para>This is used by the player animator</para>
    /// </summary>
    public void ResetCrouch()
    {
        playerMovement.isCrouching = false;
        playerAnimator.SetBool("IsCrouching", false);
    }
}
