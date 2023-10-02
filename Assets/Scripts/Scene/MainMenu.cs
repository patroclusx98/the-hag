using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public AudioManager audioManager;

    public void StartGame()
    {
        audioManager.PlaySound("Menu_Sound_Start");
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
