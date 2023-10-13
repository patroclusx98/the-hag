using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;
    public Player player;

    // Called by the player class
    public void SetIsGrounded(bool isGrounded)
    {
        player.isGrounded = isGrounded;
        animator.SetBool("IsGrounded", isGrounded);
    }

    // Called by the player class
    public void SetIsWalking(bool isWalking)
    {
        player.isWalking = isWalking;
        player.isRunning = !isWalking && player.isRunning;
    }

    // Called by the player class
    public void SetIsRunning(bool isRunning)
    {
        player.isRunning = isRunning;
        player.isWalking = !isRunning && player.isWalking;
    }

    // Called by the climbable class
    public void SetIsClimbing(bool isClimbing)
    {
        player.isClimbing = isClimbing;
        player.isWalking = !isClimbing && player.isWalking;
        player.isRunning = !isClimbing && player.isRunning;
    }

    // Called by the player class
    public void SetJumping()
    {
        player.isJumping = true;
        animator.SetBool("IsJumping", true);
    }

    // Called by the player animation event
    public void ResetJumping()
    {
        player.isJumping = false;
        animator.SetBool("IsJumping", false);
    }

    // Called by the player class
    public void SetCrouching()
    {
        player.isCrouching = true;
        animator.SetBool("IsCrouching", true);
    }

    // Called by the player animation event
    public void SetFullyCrouched()
    {
        player.hasFullyCrouched = true;
        animator.SetBool("HasFullyCrouched", true);
    }

    // Called by the player animation event
    public void ResetCrouching()
    {
        player.isCrouching = false;
        animator.SetBool("IsCrouching", false);
    }

    // Called by the player class
    public void ResetFullyCrouched()
    {
        player.hasFullyCrouched = false;
        animator.SetBool("HasFullyCrouched", false);
    }

    // Called by the player class
    public void SetImpactVelocity(float impactVelocity)
    {
        animator.SetFloat("ImpactVelocity", impactVelocity);
    }
}
