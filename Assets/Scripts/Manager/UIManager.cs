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



    
}
