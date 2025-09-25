using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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
    private Tween fadeTween;
    public void PlayBGM(int index)
    {
        if (bgmAudioSource.clip == bgmClips[index])
        {
            return;
        }
        if (fadeTween != null && fadeTween.IsActive()) fadeTween.Kill();

        bgmAudioSource.clip = bgmClips[index];
        bgmAudioSource.volume = 0;
        bgmAudioSource.Play();

        fadeTween = bgmAudioSource.DOFade(1, 1f);
    
    }
    public void FadeOutBGM()
    {
        if (fadeTween != null && fadeTween.IsActive()) fadeTween.Kill();
        fadeTween = bgmAudioSource.DOFade(0, 2f).OnComplete(() => bgmAudioSource.Stop());
    }

    
    
}
