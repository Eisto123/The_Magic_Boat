using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using Unity.VisualScripting;
using UnityEngine;

public class TeleportBoat : MonoBehaviour
{
    public GameObject teleportStonePrefab;
    private GameObject teleportStone;
    public Vector3 teleportStoneOffset = new Vector3(0, 0.1f, 0);
    public SnapInteractor snapInteractor;
    public SnapInteractable boatDock;

    public ObjectEventSO OnBoatPickedUpEvent;
    public ObjectEventSO OnBoatReleasedEvent;

    public void ResetPosition()
    {
        snapInteractor.SetComputeCandidateOverride(() => boatDock);
        snapInteractor.SetComputeShouldSelectOverride(() => true);

    }
    public void OnBoatPickedUp()
    {
        OnBoatPickedUpEvent.RaiseEvent(null, this);
        HideTeleportStone();
    }
    public void OnBoatReleased()
    {
        StartCoroutine(WaitForBoatRelease());
    }
    private IEnumerator WaitForBoatRelease()
    {
        yield return new WaitForSeconds(0.1f);
        if (snapInteractor.SelectedInteractable == null || snapInteractor.SelectedInteractable == boatDock)
        {
            OnBoatReleasedEvent.RaiseEvent(null, this);
            ResetPosition();
        }
        else
        {
            ShowTeleportStone();
        }

    }
    public void ShowTeleportStone()
    {
        teleportStone = Instantiate(teleportStonePrefab, transform.position + teleportStoneOffset, Quaternion.identity);
    }
    public void HideTeleportStone()
    {
        if (teleportStone == null) return;
        teleportStone?.GetComponent<TeleportStone>().HideStone();
    }



}
