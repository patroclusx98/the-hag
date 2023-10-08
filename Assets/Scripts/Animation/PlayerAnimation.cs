using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator playerAnimator;
    public Player player;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isGrounded"></param>
    public void SetIsGrounded(bool isGrounded)
    {
        playerAnimator.SetBool("IsGrounded", isGrounded);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="impactVelocity"></param>
    public void SetImpactVelocity(float impactVelocity)
    {
        playerAnimator.SetFloat("ImpactVelocity", impactVelocity);
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetJumping()
    {
        playerAnimator.SetBool("IsJumping", true);
    }

    /// <summary>
    /// Resets the jump parameters for the player and player animator
    /// <para>This is called by the player animator</para>
    /// </summary>
    public void ResetJumping()
    {
        player.isJumping = false;
        playerAnimator.SetBool("IsJumping", false);
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetCrouching()
    {
        playerAnimator.SetBool("IsCrouching", true);
    }

    /// <summary>
    /// Resets the crouch parameters for the player and player animator
    /// <para>This is called by the player animator</para>
    /// </summary>
    public void ResetCrouching()
    {
        player.isCrouching = false;
        playerAnimator.SetBool("IsCrouching", false);
    }

    /// <summary>
    /// 
    /// <para>This is called by the player animator</para>
    /// </summary>
    public void SetFullyCrouched()
    {
        player.hasFullyCrouched = true;
        playerAnimator.SetBool("HasFullyCrouched", true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void ResetFullyCrouched()
    {
        playerAnimator.SetBool("HasFullyCrouched", false);
    }
}
