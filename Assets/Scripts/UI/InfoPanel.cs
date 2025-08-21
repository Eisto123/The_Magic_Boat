using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class InfoPanel: MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public VideoPlayer videoPlayer;

    public List<VideoClip> videoClips;

    public void SetupUI(string title, string description)
    {
        if (titleText != null)
        {
            titleText.text = title;
        }

        if (descriptionText != null)
        {
            descriptionText.text = description;
        }


    }
}
