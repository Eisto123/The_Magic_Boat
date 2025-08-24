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
    private ProgressInfo currentProgress;
    private int currentProgressIndex = 0;


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
    void Start()
    {
        ResetCollectables();
        if (tutorialSequenceData != null && tutorialSequenceData.progressInfos.Count > 0)
        {
            currentProgress = tutorialSequenceData.progressInfos[0];
            InvokeTutorialProgress(currentProgress);
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

}
