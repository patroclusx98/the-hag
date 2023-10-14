using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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

    public void CreateSources(GameObject gameObject, AudioMixerGroup audioMixerGroup)
    {
        for (int i = 0; i < audioClips.Count; i++)
        {
            sources.Add(gameObject.AddComponent<AudioSource>());
            sources[i].hideFlags = HideFlags.HideInInspector;
            sources[i].clip = audioClips[i];
            sources[i].outputAudioMixerGroup = audioMixerGroup;
            sources[i].volume = volume;
            sources[i].pitch = pitch;
            sources[i].loop = false;
            sources[i].playOnAwake = false;
            sources[i].spatialBlend = spatialBlend;
            sources[i].maxDistance = maxDistance;
            sources[i].dopplerLevel = dopplerLevel;
        }
    }
}
