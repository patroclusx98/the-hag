using UnityEngine;

public class IntroScene : MonoBehaviour
{
    public SceneLoader sceneLoader;

    // Update is called once per frame
    private void Update()
    {
        /** Skip the intro scene **/
        if (Input.GetKeyDown(KeyCode.Space) && !sceneLoader.isSceneSkipped)
        {
            sceneLoader.isSceneSkipped = true;
            EndIntro();
        }
    }

    public void EndIntro()
    {
        sceneLoader.LoadNextScene();
    }
}
