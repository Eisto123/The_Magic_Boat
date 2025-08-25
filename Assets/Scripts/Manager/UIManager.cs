using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIManger : MonoBehaviour
{
    public static UIManger Instance;

    [Header("VR Position References")]
    public Transform leftHand;
    public Transform rightHand;
    public Transform headTransform;

    [Header("UI Elements")]
    public GameObject scanUI;
    public GameObject summonUI;
    public InfoPanel tutorialPanel;
    public Transform panelCenterPos;
    public Transform panelLeftPos;
    private Transform cameraTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        cameraTransform = Camera.main.transform;
        HideAllUI();
    }

    public void HideAllUI()
    {
        if (scanUI != null)
        {
            scanUI.SetActive(false);
        }
        if (summonUI != null)
        {
            summonUI.SetActive(false);
        }
    }

    public void DisplaySummonUI()
    {
        if (summonUI != null)
        {
            summonUI.SetActive(true);
            summonUI.transform.LookAt(cameraTransform);
        }
    }

    public void UpdateTutorialPanel(object progress)
    {
        ProgressInfo progressInfo = progress as ProgressInfo;
        
        if (progressInfo != null)
        {
            if (!progressInfo.RequireUIPanel)
            {
                tutorialPanel.gameObject.SetActive(false);
                return;
            }
            tutorialPanel.gameObject.SetActive(true);
            if (progressInfo.ProgressIndex == 0)
            {
                tutorialPanel.transform.position = panelCenterPos.position;
                tutorialPanel.transform.rotation = panelCenterPos.rotation;
            }
            else
            {
                if (tutorialPanel.transform.position != panelLeftPos.position)
                {
                    tutorialPanel.transform.DOMove(panelLeftPos.position, 1f).SetEase(Ease.OutBack);
                    tutorialPanel.transform.DORotateQuaternion(panelLeftPos.rotation, 1f).SetEase(Ease.OutBack);
                }
            
            }
            tutorialPanel.SetupUI(progressInfo.ProgressName, progressInfo.ProgressDescription);
        }
        else
        {
            Debug.LogWarning("ProgressInfo is null in UpdateTutorialPanel.");
        }
    }



    
}
