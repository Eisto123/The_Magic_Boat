using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRigManager : MonoBehaviour
{
    public bool followBoat = false;
    public Transform sittingPoint;

    void LateUpdate()
    {

        if (followBoat)
        {
            transform.position = sittingPoint.position;
        }
        if (transform.up.y < 0)
        {
            SceneLoadManager.instance.ToggleScene();
        }
    }

    public void SetFollowBoat(bool state)
    {
        followBoat = state;
    }
}
