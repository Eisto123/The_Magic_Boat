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
    public StringEventSO tutorialCompleteEvent;

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

    public void HideTutorialPanel()
    {
        tutorialPanel.gameObject.SetActive(false);
    }
    private void ProceedProcess(string progressName)
    {
        tutorialCompleteEvent.RaiseEvent(progressName, this);
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

            tutorialPanel.proceedbutton.onClick.RemoveAllListeners();

            if (progressInfo.ProceedWithButton)
            {
                tutorialPanel.proceedbutton.onClick.AddListener(() =>
                {
                    ProceedProcess(progressInfo.ProgressName);
                });
            }

            tutorialPanel.gameObject.SetActive(true);
            tutorialPanel.transform.position = panelCenterPos.position;
            tutorialPanel.transform.rotation = panelCenterPos.rotation;
            if (progressInfo.ProgressIndex == 1)
            {
                tutorialPanel.SetupUI(progressInfo.ProgressName, progressInfo.ProgressDescription,true,0);
            }
            else if (progressInfo.ProgressIndex == 2)
            {
                tutorialPanel.SetupUI(progressInfo.ProgressName, progressInfo.ProgressDescription, true, 1);
            }
            else if (progressInfo.ProgressIndex == 3)
            {
                tutorialPanel.SetupUI(progressInfo.ProgressName, progressInfo.ProgressDescription, true, 2);
            }
            else
                tutorialPanel.SetupUI(progressInfo.ProgressName, progressInfo.ProgressDescription, false);
        }
        else
        {
            Debug.LogWarning("ProgressInfo is null in UpdateTutorialPanel.");
        }
    }



    
}
