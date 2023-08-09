using UnityEngine;

[System.Serializable]
public class AudioSound
{
    [Header("Sound Attributes")]
    public string soundName;
    public AudioClip audioClip;

    [Header("Audio Attributes")]
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;
    public bool loop;
    public bool playOnAwake;

    [Header("3D Audio Attributes")]
    [Range(0f, 1f)]
    public float spatialBlend;
    [Range(1f, 30f)]
    public float maxDistance;

    [HideInInspector]
    public AudioSource source;
}
