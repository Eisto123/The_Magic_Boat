
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TutorialSequence", menuName = "ScriptableObjects/TutorialSequence", order = 1)]
public class TutorialSequenceData : ScriptableObject
{
    public List<ProgressInfo> progressInfos;
}

[System.Serializable]
public class ProgressInfo
{
    public string ProgressName;
    public string ProgressDescription;
    public bool IsCompleted;
    public bool RequireUIPanel;
    public bool RequireAudio;
}

