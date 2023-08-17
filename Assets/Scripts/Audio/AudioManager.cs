using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Manager Attributes")]
    public AudioMusic[] musics;
    public AudioSound[] sounds;
    public AudioCollection[] soundCollections;

    private AudioMixer audioMixer;
    private AudioMixerGroup soundAudioGroup;
    private AudioMixerGroup musicAudioGroup;

    // Awake is called on script load
    void Awake()
    {
        audioMixer = Resources.Load<AudioMixer>("MasterAudioMixer");
        soundAudioGroup = audioMixer.FindMatchingGroups("Master/Sound")[0];
        musicAudioGroup = audioMixer.FindMatchingGroups("Master/Music")[0];

        foreach (AudioMusic m in musics)
        {
            m.source = gameObject.AddComponent<AudioSource>();
            m.source.clip = m.audioClip;
            m.source.outputAudioMixerGroup = musicAudioGroup;

            m.source.volume = m.volume * 0.5f;
            m.source.pitch = m.pitch;
            m.source.loop = m.loop;
            m.source.playOnAwake = m.playOnAwake;

            m.source.dopplerLevel = 0f;
        }

        foreach (AudioSound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;
            s.source.outputAudioMixerGroup = soundAudioGroup;

            s.source.volume = s.volume * 0.5f;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;

            s.source.dopplerLevel = 0.05f;
        }

        foreach (AudioCollection sc in soundCollections)
        {
            for (int i = 0; i < sc.audioClips.Length; i++)
            {
                sc.collectionSources.Add(gameObject.AddComponent<AudioSource>());
                sc.collectionSources[i].clip = sc.audioClips[i];
                sc.collectionSources[i].outputAudioMixerGroup = soundAudioGroup;

                sc.collectionSources[i].volume = sc.volume * 0.5f;
                sc.collectionSources[i].pitch = sc.pitch;
                sc.collectionSources[i].playOnAwake = false;

                sc.collectionSources[i].dopplerLevel = 0.05f;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Play all onAwake music
        foreach (AudioMusic music in musics)
        {
            if (music.source.playOnAwake)
            {
                PlayMusic(music.musicName);
            }
        }

        // Play all onAwake sounds
        foreach (AudioSound sound in sounds)
        {
            if (sound.source.playOnAwake)
            {
                PlaySound2D(sound.soundName, false, 0f);
            }
        }
    }

    /** SOUND 2D METHODS **/

    public void PlaySound2D(string soundName, bool canOverlap, float frequencyInSeconds)
    {
        AudioSound s = Array.Find(sounds, sound => sound.soundName == soundName);

        if (s != null)
        {
            if (frequencyInSeconds == 0f || FrequencyTimer.GetInstance(soundName, gameObject).IsStartPhase(canOverlap ? frequencyInSeconds : frequencyInSeconds + s.audioClip.length))
            {
                if (!canOverlap)
                {
                    if (!s.source.isPlaying)
                    {
                        s.source.Play();
                    }
                }
                else
                {
                    if (!s.source.loop)
                    {
                        s.source.PlayOneShot(s.source.clip);
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

    public void PlayCollectionSound2D(string soundCollectionName, bool canOverlap, float frequencyInSeconds)
    {
        AudioCollection sc = Array.Find(soundCollections, sound => sound.collectionName == soundCollectionName);

        if (sc != null)
        {
            int collectionLength = sc.collectionSources.ToArray().Length;
            int randomSource = UnityEngine.Random.Range(0, collectionLength);

            while (sc.lastSourcePlayed == sc.collectionSources[randomSource])
            {
                randomSource = UnityEngine.Random.Range(0, collectionLength);
            }

            if (frequencyInSeconds == 0f || FrequencyTimer.GetInstance(soundCollectionName, gameObject).IsStartPhase(canOverlap ? frequencyInSeconds : frequencyInSeconds + sc.collectionSources[randomSource].clip.length))
            {
                if (!canOverlap)
                {
                    bool scSourceIsPlaying = Array.Find(sc.collectionSources.ToArray(), sound => sound.isPlaying) != null;

                    if (!scSourceIsPlaying)
                    {
                        sc.collectionSources[randomSource].Play();
                        sc.lastSourcePlayed = sc.collectionSources[randomSource];
                    }
                }
                else
                {
                    sc.collectionSources[randomSource].PlayOneShot(sc.collectionSources[randomSource].clip);
                    sc.lastSourcePlayed = sc.collectionSources[randomSource];
                }
            }
        }
        else
        {
            Debug.LogWarning("Could not find sound collection to play: " + soundCollectionName);
        }
    }

    public void StopSound2D(string soundName)
    {
        AudioSound s = Array.Find(sounds, sound => sound.soundName == soundName);
        AudioCollection sc = Array.Find(soundCollections, sound => sound.collectionName == soundName);

        if (s != null)
        {
            if (s.source.isPlaying)
            {
                s.source.Stop();
            }
        }
        else if (sc != null)
        {
            foreach (AudioSource scSource in sc.collectionSources)
            {
                if (scSource.isPlaying)
                {
                    scSource.Stop();
                }
            }
        }
        else
        {
            Debug.LogWarning("Could not find sound or sound collection to stop: " + soundName);
        }
    }

    /** SOUND 3D METHODS **/

    public void PlaySound3D(string soundName, bool canOverlap, float frequencyInSeconds, GameObject gameObject)
    {
        AudioSound s = Array.Find(sounds, sound => sound.soundName == soundName);

        if (s != null)
        {
            if (frequencyInSeconds == 0f || FrequencyTimer.GetInstance(soundName, gameObject).IsStartPhase(canOverlap ? frequencyInSeconds : frequencyInSeconds + s.audioClip.length))
            {
                AudioSource objS = Array.Find(gameObject.GetComponents<AudioSource>(), objSound => objSound.clip == s.source.clip);

                if (objS == null)
                {
                    objS = gameObject.AddComponent<AudioSource>();
                    objS.clip = s.audioClip;
                    objS.outputAudioMixerGroup = soundAudioGroup;

                    objS.volume = s.volume * 0.5f;
                    objS.pitch = s.pitch;
                    objS.loop = s.loop;
                    objS.spatialBlend = s.spatialBlend;
                    objS.maxDistance = s.maxDistance;

                    objS.playOnAwake = false;
                    objS.dopplerLevel = 0.05f;
                }

                if (!canOverlap)
                {
                    if (!objS.isPlaying)
                    {
                        objS.Play();
                    }
                }
                else
                {
                    if (!objS.loop)
                    {
                        objS.PlayOneShot(objS.clip);
                    }
                    else
                    {
                        Debug.LogWarning("Loop sounds cannot be overlapped: " + soundName);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Could not find sound to play: " + soundName);
        }
    }

    public void PlayCollectionSound3D(string soundCollectionName, bool canOverlap, float frequencyInSeconds, GameObject gameObject)
    {
        AudioCollection sc = Array.Find(soundCollections, sound => sound.collectionName == soundCollectionName);

        if (sc != null)
        {
            int collectionLength = sc.collectionSources.ToArray().Length;
            int randomSource = UnityEngine.Random.Range(0, collectionLength);

            while (sc.lastSourcePlayed == sc.collectionSources[randomSource])
            {
                randomSource = UnityEngine.Random.Range(0, collectionLength);
            }

            if (frequencyInSeconds == 0f || FrequencyTimer.GetInstance(soundCollectionName, gameObject).IsStartPhase(canOverlap ? frequencyInSeconds : frequencyInSeconds + sc.collectionSources[randomSource].clip.length))
            {
                AudioSource objSC = Array.Find(gameObject.GetComponents<AudioSource>(), objSound => objSound.clip == sc.collectionSources[randomSource].clip);

                if (objSC == null)
                {
                    objSC = gameObject.AddComponent<AudioSource>();
                    objSC.clip = sc.collectionSources[randomSource].clip;
                    objSC.outputAudioMixerGroup = soundAudioGroup;

                    objSC.volume = sc.collectionSources[randomSource].volume * 0.5f;
                    objSC.pitch = sc.collectionSources[randomSource].pitch;
                    objSC.spatialBlend = sc.collectionSources[randomSource].spatialBlend;
                    objSC.maxDistance = sc.collectionSources[randomSource].maxDistance;

                    objSC.playOnAwake = false;
                    objSC.dopplerLevel = 0.05f;
                }

                if (!canOverlap)
                {
                    // What the f*ck?
                    bool objSCSourceIsPlaying = Array.Find(Array.FindAll(gameObject.GetComponents<AudioSource>(), objSource => Array.Find(sc.collectionSources.ToArray(), scSource => objSource.clip == scSource.clip) != null), sound => sound.isPlaying) != null;

                    if (!objSCSourceIsPlaying)
                    {
                        objSC.Play();
                        sc.lastSourcePlayed = sc.collectionSources[randomSource];
                    }
                }
                else
                {
                    objSC.PlayOneShot(objSC.clip);
                    sc.lastSourcePlayed = sc.collectionSources[randomSource];
                }
            }
        }
        else
        {
            Debug.LogWarning("Could not find sound collection to play: " + soundCollectionName);
        }
    }

    public void StopSound3D(string soundName, GameObject gameObject)
    {
        AudioSound s = Array.Find(sounds, sound => sound.soundName == soundName);
        AudioCollection sc = Array.Find(soundCollections, sound => sound.collectionName == soundName);

        if (s != null)
        {
            AudioSource objS = Array.Find(gameObject.GetComponents<AudioSource>(), objSound => objSound.clip == s.source.clip);

            if (objS != null)
            {
                if (objS.isPlaying)
                {
                    objS.Stop();
                }
            }
        }
        else if (sc != null)
        {
            foreach (AudioSource scSource in gameObject.GetComponents<AudioSource>())
            {
                if (scSource.isPlaying)
                {
                    scSource.Stop();
                }
            }
        }
        else
        {
            Debug.LogWarning("Could not find sound or sound collection to stop: " + soundName);
        }
    }

    /** MUSIC METHODS **/

    // Returns true if any music is playing in the background
    private bool IsMusicPlaying()
    {
        foreach (AudioMusic m in musics)
        {
            if (m.source.isPlaying)
            {
                return true;
            }
        }

        return false;
    }

    public void PlayMusic(string musicName)
    {
        AudioMusic m = Array.Find(musics, music => music.musicName == musicName);

        if (m != null)
        {
            if (!m.source.isPlaying)
            {
                if (!IsMusicPlaying())
                {
                    m.source.Play();
                }
                else
                {
                    SwapMusic(m.source, 3f);
                }
            }
        }
        else
        {
            Debug.LogWarning("Could not find music to play: " + musicName);
        }
    }

    public void StopMusic(string musicName)
    {
        AudioMusic m = Array.Find(musics, music => music.musicName == musicName);

        if (m != null)
        {
            if (m.source.isPlaying)
            {
                m.source.Stop();
            }
        }
        else
        {
            Debug.LogWarning("Could not find music to stop: " + musicName);
        }
    }

    /** AUDIO FADE METHODS **/

    /// <summary>
    /// Fades out the specified 2D sound
    /// </summary>
    /// <param name="soundName">The name of the sound as defined in the Audio Manager instance</param>
    /// <param name="fadeTime">Fade out time in seconds</param>
    public void FadeOutSound2D(string soundName, float fadeTime)
    {
        StartCoroutine(FadeOutSound2DCR(soundName, fadeTime));
    }

    /// <summary>
    /// Fades out the specified 3D sound
    /// </summary>
    /// <param name="soundName">The name of the sound as defined in the Audio Manager instance</param>
    /// <param name="fadeTime">Fade out time in seconds</param>
    /// <param name="gameObject">GameObject the sound is attached to</param>
    public void FadeOutSound3D(string soundName, float fadeTime, GameObject gameObject)
    {
        StartCoroutine(FadeOutSound3DCR(soundName, fadeTime, gameObject));
    }

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
    public void SwapMusic(AudioSource musicSource, float swapTime)
    {
        StartCoroutine(SwapMusicCR(musicSource, swapTime));
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

    private IEnumerator FadeOutSound2DCR(string soundName, float fadeTime)
    {
        AudioSound s = Array.Find(sounds, sound => sound.soundName == soundName);

        if (s != null)
        {
            if (s.source.isPlaying)
            {
                float startVolume = s.source.volume;

                while (fadeTime > 0f && s.source.volume > 0)
                {
                    s.source.volume -= startVolume * Time.deltaTime / fadeTime;

                    yield return null;
                }

                s.source.Stop();
                s.source.volume = startVolume;
            }
        }
        else
        {
            Debug.LogWarning("Could not find sound to fade out: " + soundName);
        }
    }

    private IEnumerator FadeOutSound3DCR(string soundName, float fadeTime, GameObject gameObject)
    {
        AudioSound s = Array.Find(sounds, sound => sound.soundName == soundName);

        if (s != null)
        {
            AudioSource objS = Array.Find(gameObject.GetComponents<AudioSource>(), objSound => objSound.clip == s.source.clip);

            if (objS != null && objS.isPlaying)
            {
                float startVolume = objS.volume;

                while (fadeTime > 0f && objS.volume > 0)
                {
                    objS.volume -= startVolume * Time.deltaTime / fadeTime;

                    yield return null;
                }

                objS.Stop();
                objS.volume = startVolume;
            }
        }
        else
        {
            Debug.LogWarning("Could not find sound to fade out: " + soundName);
        }
    }

    private IEnumerator FadeOutMusicCR(float fadeTime)
    {
        foreach (AudioMusic m in musics)
        {
            if (m.source.isPlaying)
            {
                float startVolume = m.source.volume;

                while (fadeTime > 0f && m.source.volume > 0)
                {
                    m.source.volume -= startVolume * Time.deltaTime / fadeTime;

                    yield return null;
                }

                m.source.Stop();
                m.source.volume = startVolume;
            }
        }
    }

    private IEnumerator SwapMusicCR(AudioSource musicSource, float swapTime)
    {
        StartCoroutine(FadeOutMusicCR(swapTime));

        yield return new WaitForSeconds(0.5f);

        musicSource.Play();
    }

    private IEnumerator FadeOutAllSoundCR(float fadeTime)
    {
        float currentTime = 0f;
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        audioMixer.GetFloat("SoundVolume", out float currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20f);

        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, 0.0001f, currentTime / fadeTime);
            audioMixer.SetFloat("SoundVolume", Mathf.Log10(newVol) * 20f);

            yield return null;
        }

        foreach (AudioSource audioSource in allAudioSources)
        {
            if (audioSource.outputAudioMixerGroup.name == "Sound" && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        audioMixer.SetFloat("SoundVolume", 0f);
    }
}
