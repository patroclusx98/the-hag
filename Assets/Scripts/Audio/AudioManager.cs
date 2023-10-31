using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Manager Attributes")]
    public List<AudioMusic> musicList;
    public List<AudioSound> soundList;
    public List<AudioCollection> soundCollectionList;

    private AudioMixer audioMixer;
    private AudioMixerGroup soundAudioGroup;
    private AudioMixerGroup musicAudioGroup;

    // Awake is called on script load
    void Awake()
    {
        audioMixer = Resources.Load<AudioMixer>("MasterAudioMixer");
        musicAudioGroup = audioMixer.FindMatchingGroups("Master/Music")[0];
        soundAudioGroup = audioMixer.FindMatchingGroups("Master/Sounds")[0];

        /** Initialise all music audio sources **/
        foreach (AudioMusic music in musicList)
        {
            music.CreateSource(gameObject, musicAudioGroup);
        }

        /** Initialise all sound audio sources **/
        foreach (AudioSound sound in soundList)
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
        /** Play first playOnAwake music **/
        AudioMusic music = musicList.Find((music) => music.playOnAwake);
        music?.PlayMusic();

        /** Play all playOnAwake sounds **/
        foreach (AudioSound sound in soundList)
        {
            if (sound.playOnAwake)
            {
                sound.source.Play();
            }
        }
    }

    /** MUSIC METHODS **/

    /// <summary>
    /// Gets the currently playing music at the audio manager instance
    /// </summary>
    /// <returns>The currently playing music object or null</returns>
    public AudioMusic GetCurrentlyPlayingMusic()
    {
        return musicList.Find((music) => music.IsPlaying());
    }

    /// <summary>
    /// Plays the specified music at the audio manager instance
    /// <para>Only one music can play per audio manager instance, if one is already playing it will be swapped to the new one</para>
    /// </summary>
    /// <param name="musicName">The name of the music as defined within the audio manager instance</param>
    public void PlayMusic(string musicName)
    {
        AudioMusic music = musicList.Find((music) => music.name == musicName);

        if (music != null)
        {
            AudioMusic currentMusic = GetCurrentlyPlayingMusic();

            if (currentMusic != null && currentMusic != music)
            {
                StartCoroutine(SwapMusicCR(currentMusic, music, 3f));
            }
            else if (currentMusic != music)
            {
                music.PlayMusic();
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
        AudioMusic currentMusic = GetCurrentlyPlayingMusic();
        currentMusic?.StopMusic();
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
        AudioSound sound = soundList.Find((sound) => sound.name == soundName);

        if (sound != null)
        {
            if (frequencyTime == 0f || ObjectTimer.StartTimer(soundName, gameObject, canOverlap ? frequencyTime : frequencyTime + sound.soundClip.length))
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
                    sound.source.PlayOneShot(sound.soundClip);
                }
            }
        }
        else
        {
            Debug.LogWarning("Could not find sound to play: " + soundName);
        }
    }

    /// <summary>
    /// Plays a random sound from the specified sound collection at the audio manager instance
    /// </summary>
    /// <param name="soundCollectionName">The name of the sound collection as defined within the audio manager instance</param>
    /// <inheritdoc cref="PlaySound"/>>
    public void PlayCollectionSound(string soundCollectionName, bool canOverlap = false, float frequencyTime = 0f)
    {
        AudioCollection soundCollection = soundCollectionList.Find((soundCollection) => soundCollection.name == soundCollectionName);

        if (soundCollection != null)
        {
            AudioSource soundCollectionSource = null;

            while (!soundCollectionSource || soundCollectionSource == soundCollection.lastSourcePlayed)
            {
                int randomSourceIndex = Random.Range(0, soundCollection.sources.Count);
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

    /// <summary>
    /// Stops the defined sound at the audio manager instance
    /// </summary>
    /// <param name="soundName">The name of the sound as defined within the audio manager instance</param>
    public void StopSound(string soundName)
    {
        AudioSound sound = soundList.Find((sound) => sound.name == soundName);

        if (sound != null)
        {
            sound.source.Stop();
        }
        else
        {
            Debug.LogWarning("Could not find sound to stop: " + soundName);
        }
    }

    /// <summary>
    /// Stops all playing sounds at the audio manager instance
    /// </summary>
    public void StopAllSounds()
    {
        List<AudioSound> playingSounds = soundList.FindAll((sound) => sound.source.isPlaying);

        foreach (AudioSound sound in playingSounds)
        {
            sound.source.Stop();
        }
    }

    /** LOCAL AUDIO FADE METHODS **/

    /// <summary>
    /// Fades out the currently playing music at the audio manager instance
    /// </summary>
    /// <param name="fadeTime">Fade out time in seconds</param>
    public void FadeOutMusic(float fadeTime)
    {
        AudioMusic currentMusic = GetCurrentlyPlayingMusic();

        if (currentMusic != null)
        {
            StartCoroutine(FadeOutMusicCR(currentMusic, fadeTime));
        }
    }

    /// <summary>
    /// Fades out the specified sound at the audio manager instance
    /// </summary>
    /// <param name="soundName">The name of the sound as defined in the audio manager instance</param>
    /// <param name="fadeTime">Fade out time in seconds</param>
    public void FadeOutSound(string soundName, float fadeTime)
    {
        AudioSound sound = soundList.Find((sound) => sound.name == soundName);

        if (sound != null)
        {
            if (sound.source.isPlaying)
            {
                StartCoroutine(FadeOutSoundCR(sound, fadeTime));
            }
        }
        else
        {
            Debug.LogWarning("Could not find sound to fade out: " + soundName);
        }
    }

    /** LOCAL AUDIO FADE COROUTINES **/

    private IEnumerator FadeOutMusicCR(AudioMusic music, float fadeTime)
    {
        float defaultVolume = music.volume;

        while (fadeTime > 0f && music.GetSourceVolume() > 0f)
        {
            float newVolume = music.GetSourceVolume() - defaultVolume * Time.deltaTime / fadeTime;
            music.SetSourceVolume(newVolume);

            yield return null;
        }

        music.StopMusic();
        music.SetSourceVolume(defaultVolume);
    }

    private IEnumerator SwapMusicCR(AudioMusic currentMusic, AudioMusic newMusic, float swapTime)
    {
        StartCoroutine(FadeOutMusicCR(currentMusic, swapTime));

        yield return new WaitForSeconds(swapTime);

        newMusic.PlayMusic();
    }

    private IEnumerator FadeOutSoundCR(AudioSound sound, float fadeTime)
    {
        float defaultVolume = sound.volume;

        while (fadeTime > 0f && sound.source.volume > 0f)
        {
            sound.source.volume -= defaultVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        sound.source.Stop();
        sound.source.volume = defaultVolume;
    }

    /** GLOBAL AUDIO FADE METHODS **/

    /// <summary>
    /// Fades out all playing audio for all audio manager instances
    /// </summary>
    /// <param name="fadeTime">Fade out time in seconds</param>
    public void FadeOutGlobalAudio(float fadeTime)
    {
        StartCoroutine(FadeOutGlobalAudioCR(fadeTime));
    }

    /** GLOBAL AUDIO FADE COROUTINES **/

    private IEnumerator FadeOutGlobalAudioCR(float fadeTime)
    {
        audioMixer.GetFloat("MasterVolume", out float defaultVolume);

        while (fadeTime > 0f && audioMixer.GetFloat("MasterVolume", out float currentVolume) && currentVolume > -80f)
        {
            currentVolume -= (defaultVolume + 80f) * Time.deltaTime / fadeTime;
            audioMixer.SetFloat("MasterVolume", currentVolume);

            yield return null;
        }

        foreach (AudioSource audioSource in FindObjectsOfType<AudioSource>())
        {
            audioSource.Stop();
        }

        audioMixer.SetFloat("MasterVolume", defaultVolume);
    }
}
