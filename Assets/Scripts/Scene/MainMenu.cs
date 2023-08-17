using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public AudioManager audioManager;

    public void StartGame()
    {
        audioManager.PlaySound2D("Menu_Sound_Start", false, 0f);
        sceneLoader.LoadNextScene();
    }

    public void OpenSettings()
    {
        Debug.LogWarning("Not implemented!");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
