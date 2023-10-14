using UnityEngine;
using UnityEngine.Audio;

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

    public void CreateSource(GameObject gameObject, AudioMixerGroup audioMixerGroup)
    {
        source = gameObject.AddComponent<AudioSource>();
        source.hideFlags = HideFlags.HideInInspector;
        source.outputAudioMixerGroup = audioMixerGroup;
        source.clip = audioClip;
        source.volume = volume;
        source.pitch = pitch;
        source.loop = loop;
        source.playOnAwake = playOnAwake;
        source.spatialBlend = spatialBlend;
        source.maxDistance = maxDistance;
        source.dopplerLevel = dopplerLevel;
    }
}
