using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator playerAnimator;
    public Player player;

    // Called by the player class
    public void SetIsGrounded(bool isGrounded)
    {
        player.isGrounded = isGrounded;
        playerAnimator.SetBool("IsGrounded", isGrounded);
    }

    // Called by the player class
    public void SetImpactVelocity(float impactVelocity)
    {
        playerAnimator.SetFloat("ImpactVelocity", impactVelocity);
    }

    // Called by the player class
    public void SetJumping()
    {
        player.isJumping = true;
        playerAnimator.SetBool("IsJumping", true);
    }

    // Called by the player animation event
    public void ResetJumping()
    {
        player.isJumping = false;
        playerAnimator.SetBool("IsJumping", false);
    }

    // Called by the player class
    public void SetCrouching()
    {
        player.isCrouching = true;
        playerAnimator.SetBool("IsCrouching", true);
    }

    // Called by the player animation event
    public void SetFullyCrouched()
    {
        player.hasFullyCrouched = true;
        playerAnimator.SetBool("HasFullyCrouched", true);
    }

    // Called by the player animation event
    public void ResetCrouching()
    {
        player.isCrouching = false;
        playerAnimator.SetBool("IsCrouching", false);
    }

    // Called by the player class
    public void ResetFullyCrouched()
    {
        player.hasFullyCrouched = false;
        playerAnimator.SetBool("HasFullyCrouched", false);
    }
}
