using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public AudioManager audioManager;
    public Camera menuCamera;
    public Canvas menuCanvas;

    public void StartGame()
    {
        menuCanvas.enabled = false;
        StartCoroutine(ModifyFov(8f));
        audioManager.PlaySound2D("Sound_StartGame", false, 0f);
        sceneLoader.LoadNextScene(4f);
    }
    public void OpenSettings()
    {
        Debug.Log("Not implemented yet!");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator ModifyFov(float fovSpeed)
    {
        float defaultFov = menuCamera.fieldOfView; ;
        while (true)
        {
            menuCamera.fieldOfView -= defaultFov * Time.deltaTime / fovSpeed;

            yield return null;
        }
    }
}
