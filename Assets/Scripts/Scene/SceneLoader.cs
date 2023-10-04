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
    public bool isSceneSkipped = false;

    //Awake is called on script load
    private void Awake()
    {
        /** Set the fade time parameters on the transition's animator **/
        transition.SetFloat("FadeInTime", 1f / (fadeInTime > 0f ? fadeInTime : 0.0001f));
        transition.SetFloat("FadeOutTime", 1f / (fadeOutTime > 0f ? fadeOutTime : 0.0001f));
    }

    // Start is called before the first frame update
    private void Start()
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
    /// Loads the next scene in line from the currently active scene
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

    /// <summary>
    /// Coroutine to load a game scene by it's build index
    /// <para>Automatically fades all audio, and triggers the defined transition</para>
    /// </summary>
    /// <param name="sceneIndex">The game scene's build index</param>
    /// <returns>IEnumerator routine</returns>
    private IEnumerator LoadSceneCR(int sceneIndex)
    {
        audioManager.FadeOutAllAudio(fadeOutTime);
        transition.SetTrigger("StartFadeOut");

        yield return new WaitForSeconds(fadeOutTime);

        SceneManager.LoadScene(sceneIndex);
    }
}
