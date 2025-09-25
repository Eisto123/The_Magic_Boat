using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public static GameFlowManager Instance;

    [Header("Magic Book Data")]
    public MagicBookData magicBookData;

    [Header("Tutorial Sequence Data")]
    public TutorialSequenceData tutorialSequenceData;
    public ObjectEventSO tutorialProgressEvent;
    public ProgressInfo currentProgress;
    private int currentProgressIndex = 0;

    [Header("Tutorial Step Completion Flags")]
    private bool boatStepComplete = false;
    private bool snapStepComplete = false;
    private bool teleportStoneComplete = false;
    public GameObject StartPanel;


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

        if (magicBookData == null)
        {
            Debug.LogError("MagicBookData is not assigned in GameFlowManager.");
        }

        if (tutorialSequenceData == null)
        {
            Debug.LogError("TutorialSequenceData is not assigned in GameFlowManager.");
        }
    }
    public void OnStartButtonClick()
    {
        ResetCollectables();
        if (tutorialSequenceData != null && tutorialSequenceData.progressInfos.Count > 0)
        {
            currentProgress = tutorialSequenceData.progressInfos[0];
            InvokeTutorialProgress(currentProgress);
        }
        if (StartPanel != null)
        {
            StartPanel.SetActive(false);
        }
    }

    public void OnItemCollect(object collectableData)
    {
        CollectableData data = (CollectableData)collectableData;
        Element element = data.elementType;
        for (int i = 0; i < magicBookData.bookDatas.Count; i++)
        {
            foreach (ElementCollectable collectable in magicBookData.bookDatas[i].collectables)
            {
                if (collectable.elementType == element && !collectable.isCollected)
                {
                    collectable.isCollected = true;
                    Debug.Log($"Collected: {collectable.name}");
                    break;
                }
            }
        }

    }

    public void InvokeTutorialProgress(ProgressInfo progressInfo)
    {
        if (progressInfo != null)
        {
            tutorialProgressEvent.RaiseEvent(progressInfo, this);
        }
        if (progressInfo.ProgressIndex == 1)
        {
            SceneLoadManager.instance.LoadARScene();
        }
    }
    public void UpdateTutorialProgress(string completedTaskName)
    {
        if (currentProgress.ProgressName == completedTaskName)
        {
            tutorialSequenceData.progressInfos[currentProgressIndex].IsCompleted = true;
            Debug.Log($"Progress updated: {currentProgress.ProgressName} is now completed.");
            // Move to the next progress if available
            if (currentProgressIndex < tutorialSequenceData.progressInfos.Count - 1)
            {
                currentProgressIndex++;
                currentProgress = tutorialSequenceData.progressInfos[currentProgressIndex];
                InvokeTutorialProgress(currentProgress);
            }
            else
            {
                Debug.Log("All tutorial progress completed.");
            }
        }
    }



    public void ResetCollectables()
    {
        foreach (var bookData in magicBookData.bookDatas)
        {
            foreach (var collectable in bookData.collectables)
            {
                collectable.isCollected = false;
            }
        }
        Debug.Log("All collectables have been reset.");
    }

    public void OnBoatPickedUp()
    {
        if (currentProgress.ProgressIndex == 3 && !boatStepComplete)
        {
            boatStepComplete = true;
            UpdateTutorialProgress("The Boat");
        }
    }
    public void OnSnap()
    {
        if (boatStepComplete && !snapStepComplete)
        {
            snapStepComplete = true;
            UpdateTutorialProgress("Teleport Point");
        }
    }

    public void OnTeleport()
    {
        if (snapStepComplete&& !teleportStoneComplete)
        {
            teleportStoneComplete = true;
            UpdateTutorialProgress("Teleport Stone");
        }
    }


}
