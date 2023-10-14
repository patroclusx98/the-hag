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
        musicAudioGroup = audioMixer.FindMatchingGroups("Master/Music")[0];
        soundAudioGroup = audioMixer.FindMatchingGroups("Master/Sound")[0];

        /** Initialise all music audio sources **/
        foreach (Audio music in musicList)
        {
            music.CreateSource(gameObject, musicAudioGroup);
        }

        /** Initialise all sound audio sources **/
        foreach (Audio sound in soundList)
        {
            sound.CreateSource(gameObject, soundAudioGroup);
        }

        /** Initialise all sound collection audio sources **/
        foreach (AudioCollection soundCollection in soundCollectionList)
        {
            soundCollection.CreateSources(gameObject, soundAudioGroup);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        /** Play all playOnAwake music **/
        foreach (Audio music in musicList)
        {
            if (music.source.playOnAwake)
            {
                music.source.Play();
            }
        }

        /** Play all playOnAwake sounds **/
        foreach (Audio sound in soundList)
        {
            if (sound.source.playOnAwake)
            {
                sound.source.Play();
            }
        }
    }

    /** MUSIC METHODS **/

    /// <summary>
    /// Checks if any music is playing at the audio manager instance
    /// </summary>
    /// <returns>True if any music is playing at the audio manager instance</returns>
    public bool IsMusicPlaying()
    {
        return musicList.Exists((music) => music.source.isPlaying);
    }

    /// <summary>
    /// Plays the specified music at the audio manager instance
    /// <para>Only one music can play per audio manager instance, if one is already playing it will be swapped to the new one</para>
    /// </summary>
    /// <param name="musicName">The name of the music as defined within the audio manager instance</param>
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
                    StartCoroutine(SwapMusicCR(music.source, 3f));
                }
            }
        }
        else
        {
            Debug.LogWarning("Could not find music to play: " + musicName);
        }
    }

    /// <summary>
    /// Stops the currently playing music at the audio manager instance
    /// </summary>
    public void StopMusic()
    {
        Audio music = musicList.Find((music) => music.source.isPlaying);
        music?.source.Stop();
    }

    /** SOUND METHODS **/

    /// <summary>
    /// Plays the specified sound at the audio manager instance
    /// </summary>
    /// <param name="soundName">The name of the sound as defined within the audio manager instance</param>
    /// <param name="canOverlap">
    /// Allows the audio to overlap with itself if played multiple times
    /// <para>Otherwise it waits for the sound to finish playing first before playing it again</para>
    /// </param>
    /// <param name="frequencyTime">
    /// Frequency time in seconds, determines how much time should pass between frequent plays
    /// <para>If the audio can't overlap it's audio clip length will be automatically added to frequency time</para>
    /// </param>
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
    /// Stops the defined sound at the audio manager instance
    /// </summary>
    /// <param name="soundName">The name of the sound as defined within the audio manager instance</param>
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
    /// Plays a random sound from the specified sound collection at the audio manager instance
    /// </summary>
    /// <param name="soundCollectionName">The name of the sound collection as defined within the audio manager instance</param>
    /// <param name="canOverlap">
    /// Allows the audio to overlap with itself if played multiple times
    /// <para>Otherwise it waits for the sound to finish playing first before playing it again</para>
    /// </param>
    /// <param name="frequencyTime">
    /// Frequency time in seconds, determines how much time should pass between frequent plays
    /// <para>If the audio can't overlap it's audio clip length will be automatically added to frequency time</para>
    /// </param>
    /// <param name="canRepeatSameSound">
    /// Allows the same sound clip to play multiple times in a row
    /// <para>Otherwise the same sound clip will never play twice in a row</para>
    /// </param>
    public void PlayCollectionSound(string soundCollectionName, bool canOverlap = false, float frequencyTime = 0f, bool canRepeatSameSound = false)
    {
        AudioCollection soundCollection = soundCollectionList.Find((soundCollection) => soundCollection.name == soundCollectionName);

        if (soundCollection != null)
        {
            AudioSource soundCollectionSource = null;

            while (!soundCollectionSource || (!canRepeatSameSound && soundCollectionSource == soundCollection.lastSourcePlayed))
            {
                int collectionLength = soundCollection.sources.Count;
                int randomSourceIndex = Random.Range(0, collectionLength);
                soundCollectionSource = soundCollection.sources[randomSourceIndex];
            }

            if (frequencyTime == 0f || ObjectTimer.StartTimer(soundCollectionName, gameObject, canOverlap ? frequencyTime : frequencyTime + soundCollectionSource.clip.length))
            {
                if (!canOverlap)
                {
                    bool isSoundCollectionPlaying = soundCollection.sources.Exists((soundCollectionSource) => soundCollectionSource.isPlaying);

                    if (!isSoundCollectionPlaying)
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
    /// Fades out the currently playing music at the audio manager instance
    /// </summary>
    /// <param name="fadeTime">Fade out time in seconds</param>
    public void FadeOutMusic(float fadeTime)
    {
        StartCoroutine(FadeOutMusicCR(fadeTime));
    }

    /// <summary>
    /// Fades out all playing music for all audio manager instances
    /// </summary>
    /// <param name="fadeTime">Fade out time in seconds</param>
    public void FadeOutAllMusic(float fadeTime)
    {
        StartCoroutine(FadeOutAllMusicCR(fadeTime));
    }

    /// <summary>
    /// Fades out the specified sound at the audio manager instance
    /// </summary>
    /// <param name="soundName">The name of the sound as defined in the audio manager instance</param>
    /// <param name="fadeTime">Fade out time in seconds</param>
    public void FadeOutSound(string soundName, float fadeTime)
    {
        StartCoroutine(FadeOutSoundCR(soundName, fadeTime));
    }

    /// <summary>
    /// Fades out all playing sounds for all audio manager instances
    /// </summary>
    /// <param name="fadeTime">Fade out time in seconds</param>
    public void FadeOutAllSound(float fadeTime)
    {
        StartCoroutine(FadeOutAllSoundCR(fadeTime));
    }

    /// <summary>
    /// Fades out all playing audio for all audio manager instances
    /// </summary>
    /// <param name="fadeTime">Fade out time in seconds</param>
    public void FadeOutAllAudio(float fadeTime)
    {
        StartCoroutine(FadeOutAllMusicCR(fadeTime));
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

    private IEnumerator FadeOutAllMusicCR(float fadeTime)
    {
        audioMixer.GetFloat("MusicVolume", out float defaultVolume);

        while (fadeTime > 0f && audioMixer.GetFloat("MusicVolume", out float currentVolume) && currentVolume > -80f)
        {
            currentVolume -= defaultVolume * Time.deltaTime / fadeTime;
            audioMixer.SetFloat("MusicVolume", currentVolume);

            yield return null;
        }

        foreach (AudioSource audioSource in FindObjectsOfType<AudioSource>())
        {
            if (audioSource.outputAudioMixerGroup.name == "Music")
            {
                audioSource.Stop();
            }
        }

        audioMixer.SetFloat("MusicVolume", defaultVolume);
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
            if (audioSource.outputAudioMixerGroup.name == "Sound")
            {
                audioSource.Stop();
            }
        }

        audioMixer.SetFloat("SoundVolume", defaultVolume);
    }
}
