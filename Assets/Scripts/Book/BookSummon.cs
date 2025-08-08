using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UI;

public class BookSummon : MonoBehaviour
{
    [Header("Summon Settings")]
    [SerializeField] private GameObject Book;
    [SerializeField] private float summonTime;
    [SerializeField] private Transform summonPoint;
    [SerializeField] private float RockPaperDetectionTime = 1f; // Time to hold the rock pose before detecting paper

    [Header("References")]
    [SerializeField] private ActiveStateSelector Rock;
    [SerializeField] private ActiveStateSelector Paper;

    [Header("Debug")]
    public Slider summonProgressSlider;
    private float summonTimer = 0f;
    private bool duringSummon = false;
    private bool summonComplete = false;

    void OnEnable()
    {
        // Rock.WhenSelected += OnRockPoseDetected;
        // Paper.WhenSelected += OnPaperDetected;
        // Rock.WhenUnselected += OnRockPoseUndetected;
    }

    void OnDisable()
    {
        // Rock.WhenSelected -= OnRockPoseDetected;
        // Paper.WhenSelected -= OnPaperDetected;
        // Rock.WhenUnselected -= OnRockPoseUndetected;
    }
    public void OnRockPoseDetected()
    {
        Debug.Log("Rock pose detected, starting summon...");
        if (Book != null && summonPoint != null)
        {
            if (Book.activeSelf)
            {
                Debug.LogWarning("Book is already summoned.");
                return;
            }
            StartCoroutine(SetUpSummon());

        }
        else
        {
            Debug.LogWarning("Book Prefab or Summon Point is not set.");
        }
    }
    public void OnRockPoseUndetected()
    {
        if (duringSummon)
        {
            Debug.Log("Rock pose undetected, cancelling summon...");
            StopAllCoroutines();
            ResetSummon();
            return;
        }
        if (summonComplete)
        {
            StartCoroutine(WaitForPaperDetection());
        }
    }

    private IEnumerator WaitForPaperDetection()
    {
        float detectionTimer = 0f;
        while (detectionTimer < RockPaperDetectionTime)
        {
            detectionTimer += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        if (!Book.activeSelf)
        {
            Debug.Log("Paper not detected in time, resetting summon.");
            ResetSummon();
            yield break;
        }
    }

    private IEnumerator SetUpSummon()
    {
        summonTimer = 0f;
        duringSummon = true;
        UIManger.Instance.DisplaySummonUI();
        while (summonTimer < summonTime)
        {
            summonTimer += Time.deltaTime;
            if (summonProgressSlider != null)
            {
                summonProgressSlider.value = summonTimer / summonTime;
            }
            yield return null; // Wait for the next frame
        }

        summonComplete = true;
        duringSummon = false;
    }

    public void OnPaperDetected()
    {
        Debug.Log("paper pose detected");
        if (duringSummon)
        {
            Debug.Log("Paper detected during summon, cancelling...");
            StopAllCoroutines();
            ResetSummon();
            return;
        }
        if (summonComplete)
        {
            Book.SetActive(true);
            Book.transform.position = summonPoint.position + summonPoint.forward * 0.4f;
            Book.transform.LookAt(summonPoint);
            Book.transform.rotation = Quaternion.Euler(15, Book.transform.rotation.eulerAngles.y, 0);
            Debug.Log("Book summoned successfully!");
            summonComplete = false; // Reset for next summon
            UIManger.Instance.HideAllUI();
        }
    }

    private void ResetSummon()
    {
        summonComplete = false;
        duringSummon = false;
        summonTimer = 0f;
        if (summonProgressSlider != null)
        {
            summonProgressSlider.value = 0f; // Reset the slider
            UIManger.Instance.HideAllUI(); // Hide the UI after reset
        }
    }
    
}
