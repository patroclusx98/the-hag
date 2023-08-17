using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioCollection
{
    [Header("Collection Attributes")]
    public string collectionName;
    public AudioClip[] audioClips;

    [Header("Audio Attributes")]
    [Range(0f, 1f)]
    public float volume;
    [Range(0.1f, 3f)]
    public float pitch;

    [Header("3D Audio Attributes")]
    [Range(0f, 1f)]
    public float spatialBlend;
    [Range(1f, 30f)]
    public float maxDistance;

    [HideInInspector]
    public List<AudioSource> sources;
    [HideInInspector]
    public AudioSource lastSourcePlayed;
}
