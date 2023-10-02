using UnityEngine;

[System.Serializable]
public class Audio
{
    [Header("Base Attributes")]
    public string name;
    public AudioClip audioClip;

    [Header("Audio Attributes")]
    [Range(0f, 1f)]
    public float volume;
    [Range(-3f, 3f)]
    public float pitch;
    public bool loop;
    public bool playOnAwake;

    [Header("3D Audio Attributes")]
    [Range(0f, 1f)]
    public float spatialBlend;
    [Range(1f, 500f)]
    public float maxDistance;
    [Range(0f, 5f)]
    public float dopplerLevel;

    [HideInInspector]
    public AudioSource source;
}
