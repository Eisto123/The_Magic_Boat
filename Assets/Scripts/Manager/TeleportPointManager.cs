using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPointManager : MonoBehaviour
{
    public GameObject teleportPointPrefab;
    public List<GameObject> waterLevels = new List<GameObject>();
    private List<GameObject> teleportPointsHolder = new List<GameObject>();

    
    public void DisplayTeleportPoint(object index)
    {
        if(teleportPointsHolder.Count > 0)
        {
            return; // Teleport points already displayed, no need to recreate them
        }
        int idx = (int)index;
        if (idx >= 0 && idx < waterLevels.Count)
        {
            List<Transform> teleportPoints = new List<Transform>(waterLevels[idx].GetComponentsInChildren<Transform>());
            for (int i = 1; i < teleportPoints.Count; i++)
            {
                Transform tp = teleportPoints[i];
                GameObject teleportPoint = Instantiate(teleportPointPrefab, tp.position, Quaternion.identity, tp);
                teleportPoint.GetComponent<SnapPoint>().snapPointID = i - 1;
                teleportPointsHolder.Add(teleportPoint);
            }
        }
        else
        {
            Debug.LogWarning("Invalid index for teleport point: " + index);
        }
    }
    public void HideTeleportPoint(object index)
    {
        foreach (GameObject tp in teleportPointsHolder)
        {
            Destroy(tp);
        }
        teleportPointsHolder.Clear();
    }
}
