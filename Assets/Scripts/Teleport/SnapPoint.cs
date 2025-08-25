using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class SnapPoint : MonoBehaviour
{
    public int snapPointID;
    public bool isBoatDock = false;
    public bool isOccupied = false;
    public ObjectEventSO OnTeleportPointSelected;
    public ObjectEventSO OnTeleportPointVacated;

    public GameObject InfoPanel;
    public TMP_Text WaterLevelText;
    public TMP_Text SnapPointText;

    private Transform followTarget;
    bool hasTargetSetup = false;

    void OnEnable()
    {
        InfoPanel.transform.DOScale(Vector3.one, 1f).From(Vector3.zero).SetEase(Ease.OutBack);

        if (isBoatDock)
        {
            WaterLevelText.text = "Boat Dock";
            SnapPointText.text = "";
            return;
        }
        if (WaterLevelText != null)
        {
            switch (SceneLoadManager.mapIndex)
            {
                case 0:
                    WaterLevelText.text = "Shrine";
                    break;
                case 1:
                    WaterLevelText.text = "Forest";
                    break;
                case 2:
                    WaterLevelText.text = "Cave";
                    break;
                default:
                    WaterLevelText.text = "Bottom Sea";
                    break;
            }

        }
        if (SnapPointText != null)
        {
            SnapPointText.text = "Snap Point: " + (snapPointID + 1);
        }
    }
    void Update()
    {
        if (InfoPanel != null && Camera.main != null)
        {
            Vector3 camPos = Camera.main.transform.position;
            Vector3 panelPos = InfoPanel.transform.position;
            Vector3 lookDir = panelPos - camPos; // Flip the direction
            lookDir.y = 0; // Ignore vertical difference
            if (lookDir.sqrMagnitude > 0.001f)
                InfoPanel.transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
        }

    }
    void LateUpdate()
    {
        if (followTarget != null && !hasTargetSetup)
        {
            transform.position = followTarget.position;
        }
    }

    public void OnOccupied()
    {
        isOccupied = true;
        OnTeleportPointSelected.RaiseEvent(snapPointID, this);
        InfoPanel.transform.DOScale(Vector3.zero, 1f).From(Vector3.one).SetEase(Ease.OutBack);
    }
    public void OnVacated()
    {
        isOccupied = false;
        OnTeleportPointVacated.RaiseEvent(snapPointID, null);
        InfoPanel.transform.DOScale(Vector3.one, 1f).From(Vector3.zero).SetEase(Ease.OutBack);
    }

    public void Initializate(Transform target)
    {
        followTarget = target;
        hasTargetSetup = true;
    }

}
