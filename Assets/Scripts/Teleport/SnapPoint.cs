using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    public int snapPointID;
    public bool isOccupied = false;
    public ObjectEventSO OnTeleportPointSelected;
    public ObjectEventSO OnTeleportPointVacated;

    public void OnOccupied()
    {
        isOccupied = true;
        OnTeleportPointSelected.RaiseEvent(snapPointID, this);
    }
    public void OnVacated()
    {
        isOccupied = false;
        OnTeleportPointVacated.RaiseEvent(snapPointID, null);
    }

}
