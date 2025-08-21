using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialAudioMananger : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> tutorialAudioClips;

    public void PlayAudioClip(object data)
    {
        var progressInfo = data as ProgressInfo;
        if (progressInfo != null && progressInfo.RequireAudio)
        {
            if (progressInfo.ProgressIndex < tutorialAudioClips.Count)
            {
                AudioClip clipToPlay = tutorialAudioClips[progressInfo.ProgressIndex];
                if (clipToPlay != null)
                {
                    audioSource.Stop();
                    audioSource.clip = clipToPlay;
                    audioSource.Play();
                }
                else
                {
                    Debug.LogWarning($"Audio clip for progress index {progressInfo.ProgressIndex} is null.");
                }
            }
            else
            {
                Debug.LogWarning($"Progress index {progressInfo.ProgressIndex} exceeds the number of available audio clips.");
            }
        }
        else
        {
            Debug.LogWarning("Progress info is null or does not require audio.");
        }
    }
}
