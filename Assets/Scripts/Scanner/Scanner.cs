using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [Header("Volume Settings")]
    public float volumeDepth = 100f;
    public LayerMask interactableMask;

    [Header("Debug")]
    public bool drawDebugBox = true;

    public int resolution = 10;
    public Transform pointATransform;
    public Transform pointBTransform;
    public Camera vrCamera;
    private bool isScanning = false;
    private HashSet<Collectable> previouslyDetected = new HashSet<Collectable>();
    public void Scan()
    {
        isScanning = true;

        // Implement the scanning logic here
        Debug.Log("Scanning...");
    }

    void Update()
    {
        if (isScanning)
        {
            DetectObjectsInVolume(pointATransform, pointBTransform);
        }
    }

    public void StopScan()
    {
        isScanning = false;
        foreach (Collectable collectable in previouslyDetected)
        {
            collectable.isCollected = false; // Reset collection status
        }
        Debug.Log("Stopping scan...");
    }

    void DetectObjectsInVolume(Transform leftHand, Transform rightHand)
    {
        
        Vector3 center = (leftHand.position + rightHand.position) * 0.5f;
        Vector3 right = leftHand.right.normalized;
        Vector3 up = Vector3.Cross(right, vrCamera.transform.forward).normalized;
        Vector3 forward = vrCamera.transform.forward.normalized;

        float initialWidth = Vector3.Dot(rightHand.position - leftHand.position, right);
        float initialHeight = Vector3.Dot(rightHand.position - leftHand.position, up);
        float heightWidthPercentage = initialWidth / initialHeight;

        //Debug.Log($"Initial Width: {initialWidth}, Initial Height: {initialHeight}, Height/Width Percentage: {heightWidthPercentage}");
        

        float depthDistance = volumeDepth / resolution;
        float k = Mathf.Tan(vrCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float centerDistance = Vector3.Distance(center, vrCamera.transform.position);
        float heightPersentage = initialHeight / (centerDistance * k);

        List<Collider> hits = new List<Collider>();

        for (int i = 1; i < resolution; i++)
        {
            float currentDepth = i * depthDistance;

            float boxHeight = currentDepth * k * heightPersentage;
            float boxWidth = boxHeight * heightWidthPercentage;

            Vector3 scanCenter = center + vrCamera.transform.forward * currentDepth;
            Vector3 halfExtents = new Vector3(boxWidth / 2f, boxHeight / 2f, depthDistance / 2f);
            Quaternion rotation = Quaternion.LookRotation(vrCamera.transform.forward, up);

            // GameObject box = Instantiate(debugBoxPrefab, scanCenter, rotation);
            // box.transform.localScale = halfExtents * 2f;

            // Perform overlap box
            Collider[] boxHits = Physics.OverlapBox(scanCenter, halfExtents, rotation, interactableMask);
            hits.AddRange(boxHits);
        }

        HashSet<Collectable> currentlyDetected = new HashSet<Collectable>();

        foreach (Collider col in hits)
        {
            Collectable collectable = col.GetComponent<Collectable>();
            if (collectable != null)
            {
                currentlyDetected.Add(collectable);
                collectable.isCollected = true; // Mark as collected
            }
        }

        // Set previously detected but now undetected back to default
        foreach (Collectable old in previouslyDetected)
        {
            if (!currentlyDetected.Contains(old))
            {
                old.isCollected = false; // Reset collection status
            }
        }

        previouslyDetected = currentlyDetected;
    }

}
