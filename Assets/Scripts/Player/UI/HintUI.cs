using UnityEngine;
using TMPro;

public class HintUI : MonoBehaviour
{
    public static HintUI instance;
    public Animator hintAnimator;
    public TMP_Text tmpText;

    //Awake is called on script load
    private void Awake()
    {
        /** Singleton instance **/
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("More than one instance of Hint UI is found!");
        }
    }

    /// <summary>
    /// Sets the hint text and animator state to display the given hint message
    /// </summary>
    /// <param name="hintMessage">The hint message to display</param>
    public void DisplayHintMessage(string hintMessage)
    {
        if (!hintAnimator.GetBool("ShowHint"))
        {
            tmpText.text = hintMessage;
            hintAnimator.SetBool("ShowHint", true);
        }
    }

    /// <summary>
    /// Resets the hint text and animator state
    /// <para>This is used by the hint animator at the end of the animation cycle</para>
    /// </summary>
    public void ResetHintDisplay()
    {
        tmpText.text = "";
        hintAnimator.SetBool("ShowHint", false);
    }
}
