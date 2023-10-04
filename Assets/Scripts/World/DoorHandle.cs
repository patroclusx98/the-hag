using UnityEngine;

public class DoorHandle : MonoBehaviour
{
    public DoorInteractable doorObject;
    private Animator doorHandleAnimator;

    // Reset is called on component add/reset
    private void Reset()
    {
        /** Automatically add an animator component **/
        Animator animator = gameObject.GetComponent<Animator>();
        if (!animator) gameObject.AddComponent<Animator>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        doorHandleAnimator = gameObject.GetComponent<Animator>();
    }

    /// <summary>
    /// Resets the door handle parameters for the door handle animator
    /// </summary>
    public void ResetDoorHandleAnimation()
    {
        if (doorObject.isLeftSided)
        {
            doorHandleAnimator.SetBool("UseLeftHandle", false);
        }
        else
        {
            doorHandleAnimator.SetBool("UseRightHandle", false);
        }
    }
}
