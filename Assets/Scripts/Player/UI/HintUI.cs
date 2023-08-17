using UnityEngine;
using TMPro;

public class HintUI : MonoBehaviour
{
    public static HintUI instance;
    public Animator hintTextAnimator;
    public TMP_Text tmpText;

    //Awake is called on script load
    private void Awake()
    {
        /** Singleton instance **/

        if (instance != null)
        {
            Debug.LogWarning("More than one instance of Hint UI is found!");
        }

        instance = this;
    }

    public void DisplayHintMessage(string newHintText)
    {
        if (!hintTextAnimator.GetBool("ShowHint"))
        {
            tmpText.text = newHintText;
            hintTextAnimator.SetBool("ShowHint", true);
        }
    }

    public void ResetHintDisplay()
    {
        tmpText.text = "";
        hintTextAnimator.SetBool("ShowHint", false);
    }
}
