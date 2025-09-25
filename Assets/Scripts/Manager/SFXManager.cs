using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public List<AudioClip> sfxClips;
    public AudioSource sfxAudioSource;

    public void PlaySFX(int index)
    {
        sfxAudioSource.PlayOneShot(sfxClips[index]);
    }
    
}
