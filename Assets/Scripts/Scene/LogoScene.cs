﻿using UnityEngine;
using UnityEngine.Video;

public class LogoScene : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public VideoPlayer videoPlayer;
    public AudioSource videoAudioSource;

    private bool isSceneSkipped;

    // Start is called before the first frame update
    private void Start()
    {
        videoPlayer.loopPointReached += EndVideo;
    }

    // Update is called once per frame
    private void Update()
    {
        /** Skip the logo scene **/
        if (Input.GetKeyDown(KeyCode.Space) && !isSceneSkipped)
        {
            isSceneSkipped = true;
            EndVideo(videoPlayer);
        }
    }

    public void EndVideo(VideoPlayer videoPlayer)
    {
        videoAudioSource.mute = true;
        sceneLoader.LoadNextScene();
    }
}
