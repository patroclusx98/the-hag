﻿using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    AudioMixer audioMixer;
    AudioMixerGroup soundAudioGroup;
    AudioMixerGroup musicAudioGroup;

    public AudioMusic[] musics;
    public AudioSound[] sounds;
    public AudioCollection[] soundCollections;

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

    //Play all onAwake audio
    void Start()
    {
        foreach (AudioMusic m in musics)
        {
            if (m.source.playOnAwake)
            {
                PlayMusic(m.name);
            }
        }

        foreach (AudioSound s in sounds)
        {
            if (s.source.playOnAwake)
            {
                PlaySound2D(s.name, false, 0f);
            }
        }
    }

    public void PlaySound2D(string soundName, bool canOverlap, float frequencyInSeconds)
    {
        AudioSound s = Array.Find(sounds, sound => sound.name == soundName);

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
                        Debug.Log("Loop sounds cannot be overlapped! Sound: " + soundName);
                    }
                }
            }
        }
        else
        {
            Debug.Log("Could not find sound to play: " + soundName);
        }
    }
    public void PlaySound3D(string soundName, bool canOverlap, float frequencyInSeconds, GameObject gameObjectAttach)
    {
        AudioSound s = Array.Find(sounds, sound => sound.name == soundName);

        if (s != null)
        {
            if (frequencyInSeconds == 0f || FrequencyTimer.GetInstance(soundName, gameObjectAttach).IsStartPhase(canOverlap ? frequencyInSeconds : frequencyInSeconds + s.audioClip.length))
            {
                AudioSource objS = Array.Find(gameObjectAttach.GetComponents<AudioSource>(), objSound => objSound.clip == s.source.clip);

                if (objS == null)
                {
                    objS = gameObjectAttach.AddComponent<AudioSource>();
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
                        Debug.Log("Loop sounds cannot be overlapped! Sound: " + soundName);
                    }
                }
            }
        }
        else
        {
            Debug.Log("Could not find sound to play: " + soundName);
        }

    }

    public void PlayCollectionSound2D(string soundCollectionName, bool canOverlap, float frequencyInSeconds)
    {
        AudioCollection sc = Array.Find(soundCollections, sound => sound.collectionName == soundCollectionName);

        if (sc != null)
        {
            int collectionLenght = sc.collectionSources.ToArray().Length;
            int randomSource = UnityEngine.Random.Range(0, collectionLenght);

            while (sc.lastSourcePlayed == sc.collectionSources[randomSource])
            {
                randomSource = UnityEngine.Random.Range(0, collectionLenght);
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
            Debug.Log("Could not find sound collection to play: " + soundCollectionName);
        }
    }
    public void PlayCollectionSound3D(string soundCollectionName, bool canOverlap, float frequencyInSeconds, GameObject gameObjectAttach)
    {
        AudioCollection sc = Array.Find(soundCollections, sound => sound.collectionName == soundCollectionName);

        if (sc != null)
        {
            int collectionLenght = sc.collectionSources.ToArray().Length;
            int randomSource = UnityEngine.Random.Range(0, collectionLenght);

            while (sc.lastSourcePlayed == sc.collectionSources[randomSource])
            {
                randomSource = UnityEngine.Random.Range(0, collectionLenght);
            }

            if (frequencyInSeconds == 0f || FrequencyTimer.GetInstance(soundCollectionName, gameObjectAttach).IsStartPhase(canOverlap ? frequencyInSeconds : frequencyInSeconds + sc.collectionSources[randomSource].clip.length))
            {
                AudioSource objSC = Array.Find(gameObjectAttach.GetComponents<AudioSource>(), objSound => objSound.clip == sc.collectionSources[randomSource].clip);

                if (objSC == null)
                {
                    objSC = gameObjectAttach.AddComponent<AudioSource>();
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
                    bool objSCSourceIsPlaying = Array.Find(Array.FindAll(gameObjectAttach.GetComponents<AudioSource>(),                 //What
                        objSource => Array.Find(sc.collectionSources.ToArray(), scSource => objSource.clip == scSource.clip) != null),  //the
                        sound => sound.isPlaying) != null;                                                                              //fuck?

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
            Debug.Log("Could not find sound collection to play: " + soundCollectionName);
        }
    }

    public void StopSound2D(string soundName)
    {
        AudioSound s = Array.Find(sounds, sound => sound.name == soundName);
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
            Debug.Log("Could not find sound or sound collection to stop: " + soundName);
        }
    }
    public void StopSound3D(string soundName, GameObject gameObjectAttach)
    {
        AudioSound s = Array.Find(sounds, sound => sound.name == soundName);
        AudioCollection sc = Array.Find(soundCollections, sound => sound.collectionName == soundName);

        if (s != null)
        {
            AudioSource objS = Array.Find(gameObjectAttach.GetComponents<AudioSource>(), objSound => objSound.clip == s.source.clip);

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
            foreach (AudioSource scSource in gameObjectAttach.GetComponents<AudioSource>())
            {
                if (scSource.isPlaying)
                {
                    scSource.Stop();
                }
            }
        }
        else
        {
            Debug.Log("Could not find sound or sound collection to stop: " + soundName);
        }
    }

    public void PlayMusic(string musicName)
    {
        AudioMusic m = Array.Find(musics, sound => sound.name == musicName);

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
                    StartCoroutine(SwapMusic(m.source, 3f));
                }
            }
        }
        else
        {
            Debug.Log("Could not find music to play: " + musicName);
        }
    }
    public void StopMusic(string musicName)
    {
        AudioMusic m = Array.Find(musics, sound => sound.name == musicName);

        if (m != null)
        {
            if (m.source.isPlaying)
            {
                m.source.Stop();
            }
        }
        else
        {
            Debug.Log("Could not find music to stop: " + musicName);
        }
    }

    private IEnumerator SwapMusic(AudioSource musicSource, float swapSpeed)
    {
        FadeOutCurrentMusic(swapSpeed);

        yield return new WaitForSeconds(1.6f);

        musicSource.Play();
    }

    public bool IsMusicPlaying()
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

    public void FadeOutCurrentMusic(float fadeTime)
    {
        StartCoroutine(FadeOutCurrentMusicCR(fadeTime));
    }

    public void FadeOutSound2D(string audioName, float fadeTime)
    {
        StartCoroutine(FadeOutSound2DCR(audioName, fadeTime));
    }
    public void FadeOutSound3D(string audioName, float fadeTime, GameObject gameObjectAttach)
    {
        StartCoroutine(FadeOutSound3DCR(audioName, fadeTime, gameObjectAttach));
    }
    public void FadeOutAllSound(float fadeTime)
    {
        StartCoroutine(FadeOutAllSoundCR(fadeTime));
    }

    public void FadeOutAllAudio(float fadeTime)
    {
        StartCoroutine(FadeOutCurrentMusicCR(fadeTime));
        StartCoroutine(FadeOutAllSoundCR(fadeTime));
    }

    private IEnumerator FadeOutCurrentMusicCR(float fadeTime)
    {
        foreach (AudioMusic m in musics)
        {
            if (m.source.isPlaying)
            {
                float startVolume = m.source.volume;

                while (m.source.volume > 0)
                {
                    m.source.volume -= startVolume * Time.deltaTime / fadeTime;

                    yield return null;
                }

                m.source.Stop();
                m.source.volume = startVolume;

                break;
            }
        }
    }

    private IEnumerator FadeOutSound2DCR(string audioName, float fadeTime)
    {
        AudioSound s = Array.Find(sounds, sound => sound.name == audioName);

        if (s != null)
        {
            if (s.source.isPlaying)
            {
                float startVolume = s.source.volume;

                while (s.source.volume > 0)
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
            Debug.Log("Could not find audio to fade out: " + audioName);
        }
    }

    private IEnumerator FadeOutSound3DCR(string audioName, float fadeTime, GameObject gameObjectAttach)
    {
        AudioSound s = Array.Find(sounds, sound => sound.name == audioName);

        if (s != null)
        {
            AudioSource objS = Array.Find(gameObjectAttach.GetComponents<AudioSource>(), objSound => objSound.clip == s.source.clip);

            if (objS != null && objS.isPlaying)
            {
                float startVolume = objS.volume;

                while (objS.volume > 0)
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
            Debug.Log("Could not find audio to fade out: " + audioName);
        }
    }

    private IEnumerator FadeOutAllSoundCR(float fadeTime)
    {
        float currentTime = 0;
        audioMixer.GetFloat("SoundVolume", out float currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);

        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, 0.0001f, currentTime / fadeTime);
            audioMixer.SetFloat("SoundVolume", Mathf.Log10(newVol) * 20);
            yield return null;
        }

        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
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
