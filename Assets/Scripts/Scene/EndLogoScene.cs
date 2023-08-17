using UnityEngine;
using UnityEngine.Video;

public class EndLogoScene : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public VideoPlayer videoPlayer;

    // Start is called before the first frame update
    private void Start()
    {
        videoPlayer.loopPointReached += EndVideo;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !sceneLoader.isSkipped)
        {
            sceneLoader.isSkipped = true;
            EndVideo(videoPlayer);
        }
    }

    public void EndVideo(VideoPlayer videoPlayer)
    {
        sceneLoader.LoadNextScene();
    }
}
