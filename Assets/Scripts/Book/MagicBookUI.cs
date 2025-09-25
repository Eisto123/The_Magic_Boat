using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MagicBookUI : MonoBehaviour
{
    [Header("Magic Book UI Elements")]
    public GameObject bookPanel;
    public TMP_Text levelNameText;
    public TMP_Text levelDescriptionText;
    public Image levelImage;
    public TMP_Text collectablesText;
    public TMP_Text collectablesDescriptionText;
    public Image collectablesImage;
    public Toggle collectablesToggle;
    public CanvasGroup canvasGroup;

    public void FadeUI(bool fadeIn, float duration)
    {
        float targetAlpha = fadeIn ? 1f : 0f;
        canvasGroup.DOFade(targetAlpha, duration).SetEase(Ease.InOutSine);
    }
    public void UpdateBookUI(BookData bookData)
    {
        if (bookData == null) return;

        bookPanel.SetActive(true);
        levelNameText.text = bookData.levelName;
        levelDescriptionText.text = bookData.levelDescription;
        if (bookData.levelImage != null) levelImage.sprite = bookData.levelImage;

        collectablesText.text = "Collectables:";
        collectablesDescriptionText.text = "";
        collectablesImage.sprite = null;

        foreach (var collectable in bookData.collectables)
        {

            collectablesDescriptionText.text += collectable.elementType.ToString() + "\n";
            if (collectable.image != null)
            {
                collectablesImage.sprite = collectable.image;
            }
            collectablesToggle.isOn = collectable.isCollected;
        }
    }
}
