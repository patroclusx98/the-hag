using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Manager Attributes")]
    public List<Audio> musicList;
    public List<Audio> soundList;
    public List<AudioCollection> soundCollectionList;

    private AudioMixer audioMixer;
    private AudioMixerGroup soundAudioGroup;
    private AudioMixerGroup musicAudioGroup;

    // Awake is called on script load
    void Awake()
    {
        audioMixer = Resources.Load<AudioMixer>("MasterAudioMixer");
        soundAudioGroup = audioMixer.FindMatchingGroups("Master/Sound")[0];
        musicAudioGroup = audioMixer.FindMatchingGroups("Master/Music")[0];

        /** **/
        foreach (Audio music in musicList)
        {
            music.source = gameObject.AddComponent<AudioSource>();
            music.source.clip = music.audioClip;
            music.source.outputAudioMixerGroup = musicAudioGroup;
            music.source.volume = music.volume;
            music.source.pitch = music.pitch;
            music.source.loop = music.loop;
            music.source.playOnAwake = music.playOnAwake;
            music.source.spatialBlend = music.spatialBlend;
            music.source.maxDistance = music.maxDistance;
            music.source.dopplerLevel = music.dopplerLevel;
        }

        /** **/
        foreach (Audio sound in soundList)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.audioClip;
            sound.source.outputAudioMixerGroup = soundAudioGroup;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.playOnAwake = sound.playOnAwake;
            sound.source.spatialBlend = sound.spatialBlend;
            sound.source.maxDistance = sound.maxDistance;
            sound.source.dopplerLevel = sound.dopplerLevel;
        }

        /** **/
        foreach (AudioCollection soundCollection in soundCollectionList)
        {
            for (int i = 0; i < soundCollection.audioClips.Count; i++)
            {
                soundCollection.sources.Add(gameObject.AddComponent<AudioSource>());
                soundCollection.sources[i].clip = soundCollection.audioClips[i];
                soundCollection.sources[i].outputAudioMixerGroup = soundAudioGroup;
                soundCollection.sources[i].volume = soundCollection.volume;
                soundCollection.sources[i].pitch = soundCollection.pitch;
                soundCollection.sources[i].loop = false;
                soundCollection.sources[i].playOnAwake = false;
                soundCollection.sources[i].spatialBlend = soundCollection.spatialBlend;
                soundCollection.sources[i].maxDistance = soundCollection.maxDistance;
                soundCollection.sources[i].dopplerLevel = soundCollection.dopplerLevel;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        /** Play all onAwake music **/
        foreach (Audio music in musicList)
        {
            if (music.source.playOnAwake)
            {
                PlayMusic(music.name);
            }
        }

        /** Play all onAwake sounds **/
        foreach (Audio sound in soundList)
        {
            if (sound.source.playOnAwake)
            {
                PlaySound(sound.name, false, 0f);
            }
        }
    }

    /** MUSIC METHODS **/

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool IsMusicPlaying()
    {
        return musicList.Find((music) => music.source.isPlaying) != null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="musicName"></param>
    public void PlayMusic(string musicName)
    {
        Audio music = musicList.Find((music) => music.name == musicName);

        if (music != null)
        {
            if (!music.source.isPlaying)
            {
                if (!IsMusicPlaying())
                {
                    music.source.Play();
                }
                else
                {
                    SwapMusic(music.source, 3f);
                }
            }
        }
        else
        {
            Debug.LogWarning("Could not find music to play: " + musicName);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void StopMusic()
    {
        Audio music = musicList.Find((music) => music.source.isPlaying);
        music?.source.Stop();
    }

    /** SOUND METHODS **/

    /// <summary>
    /// 
    /// </summary>
    /// <param name="soundName"></param>
    /// <param name="canOverlap"></param>
    /// <param name="frequencyTime"></param>
    public void PlaySound(string soundName, bool canOverlap = false, float frequencyTime = 0f)
    {
        Audio sound = soundList.Find((sound) => sound.name == soundName);

        if (sound != null)
        {
            if (frequencyTime == 0f || ObjectTimer.StartTimer(soundName, gameObject, canOverlap ? frequencyTime : frequencyTime + sound.audioClip.length))
            {
                if (!canOverlap)
                {
                    if (!sound.source.isPlaying)
                    {
                        sound.source.Play();
                    }
                }
                else
                {
                    if (!sound.source.loop)
                    {
                        sound.source.PlayOneShot(sound.audioClip);
                    }
                    else
                    {
                        Debug.LogWarning("Looped sounds cannot be overlapped: " + soundName);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Could not find sound to play: " + soundName);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="soundName"></param>
    public void StopSound(string soundName)
    {
        Audio sound = soundList.Find((sound) => sound.name == soundName);

        if (sound != null)
        {
            if (sound.source.isPlaying)
            {
                sound.source.Stop();
            }
        }
        else
        {
            Debug.LogWarning("Could not find sound to stop: " + soundName);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="soundCollectionName"></param>
    /// <param name="canOverlap"></param>
    /// <param name="frequencyTime"></param>
    public void PlayCollectionSound(string soundCollectionName, bool canOverlap = false, float frequencyTime = 0f)
    {
        AudioCollection soundCollection = soundCollectionList.Find((soundCollection) => soundCollection.name == soundCollectionName);

        if (soundCollection != null)
        {
            AudioSource soundCollectionSource = null;

            while (!soundCollectionSource || soundCollectionSource == soundCollection.lastSourcePlayed)
            {
                int collectionLength = soundCollection.sources.Count;
                int randomSourceIndex = UnityEngine.Random.Range(0, collectionLength);
                soundCollectionSource = soundCollection.sources[randomSourceIndex];
            }

            if (frequencyTime == 0f || ObjectTimer.StartTimer(soundCollectionName, gameObject, canOverlap ? frequencyTime : frequencyTime + soundCollectionSource.clip.length))
            {
                if (!canOverlap)
                {
                    bool isSoundCollectionSourcePlaying = soundCollection.sources.Find((soundCollectionSource) => soundCollectionSource.isPlaying) != null;

                    if (!isSoundCollectionSourcePlaying)
                    {
                        soundCollectionSource.Play();
                        soundCollection.lastSourcePlayed = soundCollectionSource;
                    }
                }
                else
                {
                    soundCollectionSource.PlayOneShot(soundCollectionSource.clip);
                    soundCollection.lastSourcePlayed = soundCollectionSource;
                }
            }
        }
        else
        {
            Debug.LogWarning("Could not find sound collection to play: " + soundCollectionName);
        }
    }

    /** AUDIO FADE METHODS **/

    /// <summary>
    /// Fades out the currently playing music
    /// </summary>
    /// <param name="fadeTime">Fade out time in seconds</param>
    public void FadeOutMusic(float fadeTime)
    {
        StartCoroutine(FadeOutMusicCR(fadeTime));
    }

    /// <summary>
    /// Swaps the currently playing music to a new music with a transition
    /// </summary>
    /// <param name="musicSource">The source of the new music to play</param>
    /// <param name="swapTime">Swap cross-fade time in seconds</param>
    private void SwapMusic(AudioSource musicSource, float swapTime)
    {
        StartCoroutine(SwapMusicCR(musicSource, swapTime));
    }

    /// <summary>
    /// Fades out the specified 2D sound
    /// </summary>
    /// <param name="soundName">The name of the sound as defined in the Audio Manager instance</param>
    /// <param name="fadeTime">Fade out time in seconds</param>
    public void FadeOutSound(string soundName, float fadeTime)
    {
        StartCoroutine(FadeOutSoundCR(soundName, fadeTime));
    }

    /// <summary>
    /// Fades out all currently playing sounds
    /// </summary>
    /// <param name="fadeTime">Fade out time in seconds</param>
    public void FadeOutAllSound(float fadeTime)
    {
        StartCoroutine(FadeOutAllSoundCR(fadeTime));
    }

    /// <summary>
    /// Fades out all currently playing audio
    /// </summary>
    /// <param name="fadeTime">Fade out time in seconds</param>
    public void FadeOutAllAudio(float fadeTime)
    {
        StartCoroutine(FadeOutMusicCR(fadeTime));
        StartCoroutine(FadeOutAllSoundCR(fadeTime));
    }

    /** AUDIO FADE COROUTINES **/

    private IEnumerator FadeOutMusicCR(float fadeTime)
    {
        Audio music = musicList.Find((music) => music.source.isPlaying);

        if (music != null)
        {
            float defaultVolume = music.source.volume;

            while (fadeTime > 0f && music.source.volume > 0f)
            {
                music.source.volume -= defaultVolume * Time.deltaTime / fadeTime;

                yield return null;
            }

            music.source.Stop();
            music.source.volume = defaultVolume;
        }
    }

    private IEnumerator SwapMusicCR(AudioSource musicSource, float swapTime)
    {
        StartCoroutine(FadeOutMusicCR(swapTime));

        yield return new WaitForSeconds(0.5f);

        musicSource.Play();
    }

    private IEnumerator FadeOutSoundCR(string soundName, float fadeTime)
    {
        Audio sound = soundList.Find((sound) => sound.name == soundName);

        if (sound != null)
        {
            if (sound.source.isPlaying)
            {
                float defaultVolume = sound.source.volume;

                while (fadeTime > 0f && sound.source.volume > 0f)
                {
                    sound.source.volume -= defaultVolume * Time.deltaTime / fadeTime;

                    yield return null;
                }

                sound.source.Stop();
                sound.source.volume = defaultVolume;
            }
        }
        else
        {
            Debug.LogWarning("Could not find sound to fade out: " + soundName);
        }
    }

    private IEnumerator FadeOutAllSoundCR(float fadeTime)
    {
        audioMixer.GetFloat("SoundVolume", out float defaultVolume);

        while (fadeTime > 0f && audioMixer.GetFloat("SoundVolume", out float currentVolume) && currentVolume > -80f)
        {
            currentVolume -= defaultVolume * Time.deltaTime / fadeTime;
            audioMixer.SetFloat("SoundVolume", currentVolume);

            yield return null;
        }

        foreach (AudioSource audioSource in FindObjectsOfType<AudioSource>())
        {
            if (audioSource.outputAudioMixerGroup.name == "Sound" && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        audioMixer.SetFloat("SoundVolume", defaultVolume);
    }
}
