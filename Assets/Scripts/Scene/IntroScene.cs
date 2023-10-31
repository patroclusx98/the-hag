using UnityEngine;

public class IntroScene : MonoBehaviour
{
    public SceneLoader sceneLoader;

    private bool isSceneSkipped;

    // Update is called once per frame
    private void Update()
    {
        /** Skip the intro scene **/
        if (Input.GetKeyDown(KeyCode.Space) && !isSceneSkipped)
        {
            isSceneSkipped = true;
            EndIntro();
        }
    }

    public void EndIntro()
    {
        sceneLoader.LoadNextScene();
    }
}
