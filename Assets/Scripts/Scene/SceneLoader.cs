using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;
    public AudioManager audioManager;

    [Header("Scene Loader Inspector")]
    [ReadOnlyInspector]
    public bool isSkipped = false;

    // Update is called once per frame
    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            Cursor.visible = false;
            if (Input.GetButtonDown("Jump") && !isSkipped)
            {
                isSkipped = true;
                LoadNextScene(0f);
            }
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (Input.GetButtonDown("Jump") && !isSkipped)
            {
                isSkipped = true;
                LoadNextScene(4f);
            }
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            Cursor.visible = true;
        }
    }

    private IEnumerator LoadLevel(int index, float transitionLengthInSeconds)
    {
        transition.SetTrigger("EndFade");

        yield return new WaitForSeconds(transitionLengthInSeconds);

        SceneManager.LoadScene(index);
    }

    public void LoadNextScene(float transitionLengthInSeconds)
    {
        if (audioManager != null)
            audioManager.FadeOutAllAudio(2.5f);
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1, transitionLengthInSeconds));
    }
}
