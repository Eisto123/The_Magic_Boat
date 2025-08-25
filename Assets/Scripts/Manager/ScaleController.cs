using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class ScaleController : MonoBehaviour
{
    public GameObject grabControl;
    public Vector3 currentScale;
    public float[] scaleLevel = new float[] { 0.1f, 0.115f, 0.158f, 0.2f };
    public float[] waterLevelScale = new float[] { 1f,0.67f, 0.33f,0.12f};
    public ObjectEventSO transformEndEvent;
    private int currentLevel;
    public GameObject waterLevel;
    public Transform basePoint;
    public ObjectEventSO ModelResetCompleteEvent;
    public StringEventSO tutorialCompleteEvent;



    private Quaternion lastRotation;
    private Vector3 lastScale;

    void Awake()
    {
        currentScale = transform.localScale;
        lastRotation = transform.rotation;
        lastScale = transform.localScale;
    
    }
    void Update()
    {
        float xScale = transform.localScale.x;
        float targetWaterY = GetInterpolatedWaterLevel(xScale);
        Vector3 waterScale = waterLevel.transform.localScale;
        waterScale.y = targetWaterY;
        waterLevel.transform.localScale = waterScale;

        // --- Tutorial Step: Detect Rotation Change ---
        if (!GameFlowManager.rotationStepComplete && Quaternion.Angle(transform.rotation, lastRotation) > 10f) // 10 degrees threshold
        {
            GameFlowManager.rotationStepComplete = true;
            tutorialCompleteEvent.RaiseEvent("Ecosystem Tank Rotation",this);
        }

        // --- Tutorial Step: Detect Scale Change ---
        if (!GameFlowManager.scaleStepComplete && (transform.localScale - lastScale).sqrMagnitude > 0.001f)
        {
            GameFlowManager.scaleStepComplete = true;
            tutorialCompleteEvent.RaiseEvent("Ecosystem Tank Scale",this);
        }
    
    }
    private float GetInterpolatedWaterLevel(float xScale)
    {
        // Find which two scaleLevels xScale is between
        for (int i = 0; i < scaleLevel.Length - 1; i++)
        {
            if (xScale >= scaleLevel[i] && xScale <= scaleLevel[i + 1])
            {
                float t = Mathf.InverseLerp(scaleLevel[i], scaleLevel[i + 1], xScale);
                return Mathf.Lerp(waterLevelScale[i], waterLevelScale[i + 1], t);
            }
        }
        // If out of bounds, clamp to nearest
        if (xScale < scaleLevel[0]) return waterLevelScale[0];
        return waterLevelScale[waterLevelScale.Length - 1];
    }


    public void OnTransformEnd()
    {
        currentScale = transform.localScale;
        currentScale.x = GetClosestLevel(currentScale.x, scaleLevel);
        currentScale.z = currentScale.x;
        currentScale.y = currentScale.x / 0.1f * 0.15f;
        currentLevel = System.Array.IndexOf(scaleLevel, currentScale.x);
        transform.DOScale(currentScale, 0.2f).SetEase(Ease.OutBack);
        Debug.Log(currentLevel);
        transformEndEvent.RaiseEvent(currentLevel, this);
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

    public void ResetPosition()
    {
        transform.DOMove(basePoint.position, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            ModelResetCompleteEvent.RaiseEvent(currentLevel, this);
            //grabControl.SetActive(false);
        });
        //transform.DORotate(basePoint.rotation.eulerAngles, 0.5f).SetEase(Ease.OutBack);
    }
    public void EnableGrabbing()
    {
        grabControl.SetActive(true);
    }

}
