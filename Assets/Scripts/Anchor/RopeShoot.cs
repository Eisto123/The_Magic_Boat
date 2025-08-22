using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RopeShoot : MonoBehaviour
{
    [Header("Rope Settings")]
    [SerializeField] private Transform instanciatePoint;
    [SerializeField] private Camera vrCamera;
    [SerializeField] private LayerMask targetLayer;

    [SerializeField] private float maxDetectionLength = 20f;
    [SerializeField] private float maxRopeLength = 10f;
    private GameObject currentAnchor;
    private float maxSelectAngle = 30f; // in degrees
    public GameObject reelPrefab;
    private Reel reelInstance;
    private float scanInterval = 0.2f;
    private float scanTimer = 0f;
    private Collider[] anchorBuffer = new Collider[32];

    void FixedUpdate()
    {
        scanTimer += Time.fixedDeltaTime;
        if (scanTimer >= scanInterval)
        {
            ScanAndSelectAnchor();
            scanTimer = 0f;
        }
    }


    public void ScanAndSelectAnchor()
    {
        int count = Physics.OverlapSphereNonAlloc(vrCamera.transform.position, maxDetectionLength, anchorBuffer, targetLayer);
        GameObject bestAnchor = null;
        float bestDot = -1f;

        for (int i = 0; i < count; i++)
        {
            var anchor = anchorBuffer[i];
            anchor.TryGetComponent<Anchor>(out Anchor anchorComponent);
            if (anchorComponent != null)
                anchorComponent.SetAnchorState(AnchorState.Unselected);

            float distance = Vector3.Distance(vrCamera.transform.position, anchor.transform.position);
            if (distance > maxRopeLength) continue;

            Vector3 toAnchor = (anchor.transform.position - vrCamera.transform.position).normalized;
            float dot = Vector3.Dot(vrCamera.transform.forward, toAnchor);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
            if (angle > maxSelectAngle) continue;

            if (dot > bestDot)
            {
                bestDot = dot;
                bestAnchor = anchor.gameObject;
            }
        }

        currentAnchor = bestAnchor;
        if (currentAnchor != null)
            currentAnchor.GetComponent<Anchor>().SetAnchorState(AnchorState.Selected);
    }



    private Coroutine shootTimerCoroutine;

    public void OnShotPoseDetected()
    {
        if (currentAnchor != null)
        {
            if (reelInstance == null)
            {
                Vector3 toAnchor = (currentAnchor.transform.position - instanciatePoint.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(toAnchor, Vector3.up);
                var GO = Instantiate(reelPrefab, instanciatePoint.position, lookRotation);
                reelInstance = GO.GetComponent<Reel>();
    
            }

            if (shootTimerCoroutine != null)
                StopCoroutine(shootTimerCoroutine);
            shootTimerCoroutine = StartCoroutine(ShootTimerRoutine());
        }
    }
    public void OnMoveCompete(object sender)
    {
        if (reelInstance != null)
        {
            Destroy(reelInstance.gameObject);
            reelInstance = null;
        }
    }
    public void OnShotPoseUndetected()
    {
        if (shootTimerCoroutine != null)
        {
            StopCoroutine(shootTimerCoroutine);
            shootTimerCoroutine = null;
        }
    }
    private IEnumerator ShootTimerRoutine()
    {
        yield return new WaitForSeconds(1f);
        if(reelInstance != null)
            reelInstance.ShootRope(currentAnchor);
        shootTimerCoroutine = null;
    }




}
