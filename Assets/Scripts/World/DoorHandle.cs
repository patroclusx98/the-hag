using UnityEngine;

public class DoorHandle : MonoBehaviour
{
    public DoorInteractable doorObject;
    private Animator doorHandleAnimator;

    // Reset is called on component add/reset
    private void Reset()
    {
        /** Auto add animator **/
        Animator animator = gameObject.GetComponent<Animator>();
        if (!animator) gameObject.AddComponent<Animator>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        doorHandleAnimator = gameObject.GetComponent<Animator>();
    }

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
