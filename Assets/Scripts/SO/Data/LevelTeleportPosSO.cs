using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "LevelData/LevelTeleportPosSO", fileName = "LevelTeleportPosSO")]
public class LevelTeleportPosSO : ScriptableObject
{
    public List<TeleportPosition> teleportPositions = new List<TeleportPosition>();
}

[System.Serializable]
public class TeleportPosition
{
    public string positionName;
    public SerializeVector3 targetPos;
    public SerializeVector3 targetRotation;
}
