using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator playerAnimator;
    public PlayerMovement playerMovement;

    public void SetJump()
    {
        playerMovement.verticalVelocity.y = Mathf.Sqrt(-playerMovement.jumpHeight * playerMovement.gravityForce);
    }

    public void ResetJump()
    {
        playerMovement.isJumping = false;
        playerAnimator.SetBool("IsJumping", false);
    }

    public void SetCrouch()
    {
        playerMovement.hasCrouched = true;
        playerAnimator.SetBool("HasCrouched", true);
    }

    public void ResetCrouch()
    {
        playerMovement.isCrouching = false;
        playerAnimator.SetBool("IsCrouching", false);
    }
}
