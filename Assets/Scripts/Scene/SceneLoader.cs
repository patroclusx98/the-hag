using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;
    public AudioManager audioManager;

    bool isSkipped = false;

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Cursor.visible = false;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (Input.GetButtonDown("Jump") && !isSkipped)
            {
                LoadNextScene(4f);
                isSkipped = true;
            }
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            Cursor.visible = true;
        }
    }

    public void LoadNextScene(float transitionLengthInSeconds)
    {
        if (audioManager != null)
            audioManager.FadeOutAllAudio(2.5f);
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1, transitionLengthInSeconds));
    }

    IEnumerator LoadLevel(int index, float transitionLengthInSeconds)
    {
        transition.SetTrigger("EndFade");

        yield return new WaitForSeconds(transitionLengthInSeconds);

        SceneManager.LoadScene(index);
    }
}
