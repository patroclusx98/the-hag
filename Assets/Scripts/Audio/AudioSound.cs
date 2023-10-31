using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioSound
{
    [Header("Base Attributes")]
    public string name;
    public AudioClip soundClip;

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
        if (soundClip != null)
        {
            source = gameObject.AddComponent<AudioSource>();
            source.hideFlags = HideFlags.HideInInspector;
            source.outputAudioMixerGroup = audioMixerGroup;
            source.clip = soundClip;
            source.volume = volume;
            source.pitch = pitch;
            source.loop = loop;
            source.spatialBlend = spatialBlend;
            source.maxDistance = maxDistance;
            source.dopplerLevel = dopplerLevel;
        }
        else
        {
            Debug.LogWarning("Sound has no audio clip attached: " + name);
        }
    }
}
