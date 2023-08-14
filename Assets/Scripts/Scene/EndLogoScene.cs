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

    public void EndVideo(VideoPlayer vp)
    {
        sceneLoader.LoadNextScene(1f);
    }
}
