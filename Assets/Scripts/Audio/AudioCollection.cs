using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioCollection
{
    [Header("Base Attributes")]
    public string name;
    public List<AudioClip> audioClips;

    [Header("Audio Attributes")]
    [Range(0f, 1f)]
    public float volume;
    [Range(-3f, 3f)]
    public float pitch;

    [Header("3D Audio Attributes")]
    [Range(0f, 1f)]
    public float spatialBlend;
    [Range(1f, 500f)]
    public float maxDistance;
    [Range(0f, 5f)]
    public float dopplerLevel;

    [HideInInspector]
    public List<AudioSource> sources;
    [HideInInspector]
    public AudioSource lastSourcePlayed;
}
