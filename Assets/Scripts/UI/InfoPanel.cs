using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class InfoPanel: MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public VideoPlayer videoPlayer;
    public RawImage videoImage;
    public Button proceedbutton;
    public List<VideoClip> videoClips;

    public void SetupUI(string title, string description, bool playVideo, int videoIndex = 0)
    {
        if (titleText != null)
        {
            titleText.text = title;
        }

        if (descriptionText != null)
        {
            descriptionText.text = description;
        }
        if (playVideo)
        {

            if (videoPlayer != null && videoClips != null && videoIndex >= 0 && videoIndex < videoClips.Count)
            {
                videoImage.enabled = true;
                videoPlayer.clip = videoClips[videoIndex];
                videoPlayer.Play();
            }
        }
        else
        {
            videoImage.enabled = false;
            if (videoPlayer != null && videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
            }
        }

    }
}
