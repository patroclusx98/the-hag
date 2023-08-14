using UnityEngine;
using TMPro;

public class HintUI : MonoBehaviour
{
    public static HintUI instance;
    public Animator animator;
    public TMP_Text tmpText;

    //Awake is called on script load
    private void Awake()
    {
        //Singleton instance
        if (instance != null)
            Debug.LogWarning("More than one instance of Hint UI is found!");
        instance = this;
    }

    public void DisplayHintMessage(string newHintText)
    {
        bool canDisplay = true;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("HintAnim") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0f)
            canDisplay = false;

        if (canDisplay)
        {
            tmpText.text = newHintText;
            animator.SetTrigger("ShowHint");
        }
    }
}
