using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            tutorialPanel.SetupUI(progressInfo.ProgressName, progressInfo.ProgressDescription);
        }
        else
        {
            Debug.LogWarning("ProgressInfo is null in UpdateTutorialPanel.");
        }
    }



    
}
