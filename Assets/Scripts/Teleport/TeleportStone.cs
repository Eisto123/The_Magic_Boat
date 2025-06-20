using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class TeleportStone : MonoBehaviour
{
    public Material material;
    public float dissolveTime = 1f;
    private float fullHeight = 1f;
    private float zeroHeight = -1f;
    public ObjectEventSO StartTeleportEvent;

    void OnEnable()
    {
        material.SetFloat("_CutoffHeight", zeroHeight);
        DisplayStone();
    }
    public void DisplayStone()
    {
        StartCoroutine(DissolveEffectToFull(dissolveTime));
    }
    public void HideStone()
    {
        StartCoroutine(DissolveEffectToZero(dissolveTime));
    }
    public void StartTeleport()
    {
        StartTeleportEvent.RaiseEvent(null, this);
        StartCoroutine(DissolveEffectToZero(dissolveTime));
    }


    private IEnumerator DissolveEffectToFull(float dissolveTime)
    {
        float elapsedTime = 0f;

        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / dissolveTime);
            material.SetFloat("_CutoffHeight", math.lerp(zeroHeight, fullHeight, t));
            yield return null;
        }
        material.SetFloat("_CutoffHeight", fullHeight);
    }
    private IEnumerator DissolveEffectToZero(float dissolveTime)
    {
        float elapsedTime = 0f;

        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / dissolveTime);
            material.SetFloat("_CutoffHeight", math.lerp(fullHeight, zeroHeight, t));
            yield return null;
        }
        material.SetFloat("_CutoffHeight", zeroHeight);
        Destroy(gameObject, 1f); // Destroy after the hide animation
    }
}
