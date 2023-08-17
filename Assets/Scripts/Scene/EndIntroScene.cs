using UnityEngine;

public class EndIntroScene : MonoBehaviour
{
    public SceneLoader sceneLoader;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !sceneLoader.isSkipped)
        {
            sceneLoader.isSkipped = true;
            EndIntro();
        }
    }

    public void EndIntro()
    {
        sceneLoader.LoadNextScene();
    }
}
