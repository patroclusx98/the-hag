using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class AudioMusic
{
    [Header("Base Attributes")]
    public string name;
    public AudioClip introClip;
    public AudioClip loopClip;

    [Header("Audio Attributes")]
    [Range(0f, 1f)]
    public float volume;
    [Range(-3f, 3f)]
    public float pitch;
    public bool playOnAwake;

    [HideInInspector]
    public AudioSource introSource;
    [HideInInspector]
    public AudioSource loopSource;

    public void CreateSource(GameObject gameObject, AudioMixerGroup audioMixerGroup)
    {
        /** Intro Source **/
        if (introClip != null)
        {
            introSource = gameObject.AddComponent<AudioSource>();
            introSource.hideFlags = HideFlags.HideInInspector;
            introSource.outputAudioMixerGroup = audioMixerGroup;
            introSource.clip = introClip;
            introSource.volume = volume;
            introSource.pitch = pitch;
            introSource.loop = false;
            introSource.spatialBlend = 0f;
        }

        /** Loop Source **/
        if (loopClip != null)
        {
            loopSource = gameObject.AddComponent<AudioSource>();
            loopSource.hideFlags = HideFlags.HideInInspector;
            loopSource.outputAudioMixerGroup = audioMixerGroup;
            loopSource.clip = loopClip;
            loopSource.volume = volume;
            loopSource.pitch = pitch;
            loopSource.loop = true;
            loopSource.spatialBlend = 0f;
        }
    }

    public bool IsPlaying()
    {
        return (introSource != null && introSource.isPlaying) ||
            (loopSource != null && loopSource.isPlaying);
    }

    public void PlayMusic()
    {
        if (introSource != null)
        {
            introSource.Play();

            if (loopSource != null)
            {
                loopSource.PlayScheduled(AudioSettings.dspTime + introClip.length);
            }
        }
        else if (loopSource != null)
        {
            loopSource.Play();
        }
    }

    public void StopMusic()
    {
        if (introSource != null)
        {
            introSource.Stop();
        }

        if (loopSource != null)
        {
            loopSource.Stop();
        }
    }

    public float GetSourceVolume()
    {
        if (introSource != null)
        {
            return introSource.volume;
        }

        if (loopSource != null)
        {
            return loopSource.volume;
        }

        return 0f;
    }

    public void SetSourceVolume(float volume)
    {
        if (introSource != null)
        {
            introSource.volume = volume;
        }

        if (loopSource != null)
        {
            loopSource.volume = volume;
        }
    }
}
