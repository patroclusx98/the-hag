using UnityEngine;

[System.Serializable]
public class AudioMusic
{
    [Header("Music Attributes")]
    public string musicName;
    public AudioClip audioClip;

    [Header("Audio Attributes")]
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;
    public bool loop;
    public bool playOnAwake;

    [HideInInspector]
    public AudioSource source;
}
