using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;
    public AudioSource bgmAudioSource;
    public List<AudioClip> bgmClips;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private Coroutine fadeCoroutine;
    public void PlayBGM(int index)
    {
        if (bgmAudioSource.clip == bgmClips[index])
        {
            return;
        }
        bgmAudioSource.clip = bgmClips[index];

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeIn(bgmAudioSource, 1f));
    }
    public void FadeOutBGM()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeOut(bgmAudioSource, 2f));
    }


    private IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    private IEnumerator FadeIn(AudioSource audioSource, float fadeTime)
    {
        float targetVolume = 1;
        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += targetVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.volume = targetVolume;
    }
    
    
}
