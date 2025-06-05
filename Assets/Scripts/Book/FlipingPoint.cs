using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipingPoint : MonoBehaviour
{
    public GameObject flipingPoint;
    public Vector3 position;
    public Quaternion rotation;
    public ObjectEventSO releaseEvent;

    void Awake()
    {
        position = this.transform.localPosition;
        rotation = this.transform.localRotation;
    }
    public void ResetPosition()
    {
        //releaseEvent.RaiseEvent(null, this);
        StartCoroutine(ResetPositionCoroutine());
    }
    IEnumerator ResetPositionCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        this.transform.localPosition = position;
        this.transform.localRotation = rotation;
    }
}
