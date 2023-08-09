using UnityEngine;
using UnityEngine.SceneManagement;

public class EndIntroScene : MonoBehaviour
{
    public SceneLoader sceneLoader;

    public void EndIntro()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            sceneLoader.LoadNextScene(4f);
        }
    }
}
