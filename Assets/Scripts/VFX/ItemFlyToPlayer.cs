using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemFlyToPlayer : MonoBehaviour
{
    public Transform playerTransform;
    public GameObject collectEffectPrefab;
    public GameObject collecttedStarEffectPrefab;
    public ObjectEventSO FlyToPlayerCompleteEvent;

    public float duration = 1.5f; // Duration of the fly animation

    public void OnCollect(object collectableData)
    {
        CollectableData data = (CollectableData)collectableData;
        StartCoroutine(FlyToPlayerRoutine(data.position, playerTransform.position));
    }

    private IEnumerator FlyToPlayerRoutine(Vector3 startPosition, Vector3 endPosition)
    {
        GameObject effect = Instantiate(collectEffectPrefab, startPosition, Quaternion.identity);
        GameObject getEffect = Instantiate(collecttedStarEffectPrefab, startPosition, Quaternion.identity);
        // Make curve more dramatic and gentle
        Vector3 direction = (endPosition - startPosition).normalized;
        Vector3 randomDir = Quaternion.AngleAxis(Random.Range(-90f, 90f), Vector3.up) * direction; // wider angle
        float curveDistance = Vector3.Distance(startPosition, endPosition) * 0.7f; // longer curve
        Vector3 controlPoint = startPosition + randomDir * curveDistance + Vector3.up * Random.Range(2f, 4f); // higher arc

        float t = 0f;
        while (t < 1f)
        {
            // Ease-in-out for gentle movement
            float easedT = Mathf.SmoothStep(0f, 1f, t);

            // Speed curve: slow at start/end, faster in middle
            float speed = Mathf.Lerp(0.5f, 2.0f, Mathf.Sin(easedT * Mathf.PI));
            t += Time.deltaTime * speed / duration;

            // Quadratic Bezier formula
            Vector3 pos = Mathf.Pow(1 - easedT, 2) * startPosition +
                        2 * (1 - easedT) * easedT * controlPoint +
                        Mathf.Pow(easedT, 2) * endPosition;

            effect.transform.position = pos;
            yield return null;
        }

        effect.transform.position = endPosition;
        FlyToPlayerCompleteEvent.RaiseEvent(null, this);
        Destroy(effect);
        Destroy(getEffect);
    }
}
