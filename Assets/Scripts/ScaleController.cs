using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ScaleController : MonoBehaviour
{
    public Vector3 currentScale;
    public float[] scaleLevel = new float[] { 0.1f, 0.115f, 0.158f, 0.2f };
    public ObjectEventSO transformEndEvent;
    private int currentLevel;
    public GameObject waterLevel;
    private float waterVolume;

    void Awake()
    {
        currentScale = transform.localScale;
        waterVolume = currentScale.x * currentScale.z * waterLevel.transform.localScale.y;
    }
    void Update()
    {
        waterLevel.transform.localScale = new Vector3(waterLevel.transform.localScale.x, waterVolume / (transform.localScale.x * transform.localScale.z), waterLevel.transform.localScale.z);
        waterLevel.transform.localPosition = new Vector3(0, -(1 - waterLevel.transform.localScale.y) / 2, 0);
    }

    public void OnTransformEnd()
    {
        currentScale = transform.localScale;
        currentScale.x = GetClosestLevel(currentScale.x, scaleLevel);
        currentScale.z = currentScale.x;
        currentScale.y = currentScale.x;
        currentLevel = System.Array.IndexOf(scaleLevel, currentScale.x);
        transform.DOScale(currentScale, 0.2f).SetEase(Ease.OutBack);
        Debug.Log(currentLevel);
        transformEndEvent.RaiseEvent(currentLevel,this);
    }
    
    float GetClosestLevel(float value, float[] levels)
{
    float closest = levels[0];
    float minDistance = Mathf.Abs(value - closest);

    for (int i = 1; i < levels.Length; i++)
    {
        float distance = Mathf.Abs(value - levels[i]);
        if (distance < minDistance)
        {
            closest = levels[i];
            minDistance = distance;
        }
    }

    return closest;
}
    
}
