using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public AudioManager audioManager;

    public void StartGame()
    {
        audioManager.PlaySound("menu_start_game");
        sceneLoader.LoadNextScene();
    }

    public void OpenSettings()
    {
        Debug.LogWarning("Not yet implemented!");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
