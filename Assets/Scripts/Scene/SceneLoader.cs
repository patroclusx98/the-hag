using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator transition;
    public AudioManager audioManager;

    [Header("Scene Loader Attributes")]
    [Tooltip("Fade in time in seconds. Setting 0 will skip the fade in.")]
    public float fadeInTime = 2f;
    [Tooltip("Fade out time in seconds. Setting 0 will skip the fade out.")]
    public float fadeOutTime = 2f;

    [Header("Scene Loader Inspector")]
    [ReadOnlyInspector]
    public bool isSkipped = false;

    //Awake is called on script load
    private void Awake()
    {
        transition.SetFloat("FadeInTime", 1f / (fadeInTime > 0f ? fadeInTime : 0.0001f));
        transition.SetFloat("FadeOutTime", 1f / (fadeOutTime > 0f ? fadeOutTime : 0.0001f));
    }

    // Update is called once per frame
    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            /** Logo Scene **/

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            /** Intro Scene **/

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            /** Menu Scene **/

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            /** Game Scene **/

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// Loads the next scene with the cross-fade transition
    /// </summary>
    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (SceneUtility.GetScenePathByBuildIndex(nextSceneIndex).Length > 0)
        {
            StartCoroutine(LoadSceneCR(nextSceneIndex));
        }
        else
        {
            Debug.LogWarning("Scene does not exist for the next index: " + nextSceneIndex);
        }
    }

    private IEnumerator LoadSceneCR(int sceneIndex)
    {
        audioManager.FadeOutAllAudio(fadeOutTime);
        transition.SetTrigger("StartFadeOut");

        yield return new WaitForSeconds(fadeOutTime);

        SceneManager.LoadScene(sceneIndex);
    }
}
