using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Oculus.Interaction.HandGrab;

public class Reel : MonoBehaviour
{
    public Transform wheel;
    private Transform anchor;
    private Transform boat;
    [SerializeField] private Transform shootPoint;
    public float pullForce = 10f;
    private float lastWheelAngleX;
    private bool ropeDeployed = false;

    [SerializeField] private LineRenderer lineRenderer;
    // Add this field to control the wobble
    [SerializeField] private int ropeSegments = 20;
    [SerializeField] private float wobbleMagnitude = 0.04f;
    [SerializeField] private float wobbleFrequency = 20f;
    [SerializeField] private float completeDistance = 1.0f;
    [SerializeField] float turnSpeed = 0.5f;
    [SerializeField] private HandGrabInteractable handGrabInteractable;
    public bool moveComplete = false;
    public ObjectEventSO moveCompleteEvent;
    [SerializeField] private float maxReelDistance = 2f;
    private Coroutine wobbleCoroutine;
    private Rigidbody boatRB;
    private Coroutine destroyCoroutine;
    void OnEnable()
    {
        lastWheelAngleX = wheel.localEulerAngles.x;
        boat = GameObject.FindGameObjectWithTag("Boat").transform;
        boatRB = boat.GetComponent<Rigidbody>();
        lineRenderer.positionCount = ropeSegments;
        lineRenderer.enabled = false;
        ropeDeployed = false;
    }
    void FixedUpdate()
    {
        if (boat != null && Vector3.Distance(transform.position, boat.position) > maxReelDistance)
        {
            Destroy(gameObject);
        }
        if (handGrabInteractable.State != Oculus.Interaction.InteractableState.Select)
        {
            if (destroyCoroutine == null)
                destroyCoroutine = StartCoroutine(DestroyAfterDelay());
            return;
        }
        else
        {
            // Cancel destroy timer if grabbed again
            if (destroyCoroutine != null)
            {
                StopCoroutine(destroyCoroutine);
                destroyCoroutine = null;
            }
        }
        if (!ropeDeployed) return;

        float currentAngleX = wheel.localEulerAngles.x;
        float deltaAngle = Mathf.DeltaAngle(lastWheelAngleX, currentAngleX);

        // Anti-clockwise (negative delta): pull boat closer
        if (deltaAngle < 0 && anchor != null)
        {
            if (boatRB != null)
            {
                if (boatRB != null)
                {
                    Vector3 toAnchor = anchor.position - boat.position;
                    Vector3 flatToAnchor = new Vector3(toAnchor.x, 0, toAnchor.z);
                    Vector3 boatForward = new Vector3(boat.forward.x, 0, boat.forward.z).normalized;

                    // Pull force only in the boat's forward direction, scaled by how well it's facing the anchor
                    float forwardDot = Vector3.Dot(boatForward, flatToAnchor.normalized);
                    float forceMagnitude = Mathf.Abs(deltaAngle) * pullForce * Mathf.Max(0, forwardDot);
                    boatRB.AddForce(boatForward * forceMagnitude, ForceMode.Force);

                    if (flatToAnchor.sqrMagnitude > 0.001f && boatForward.sqrMagnitude > 0.001f)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(flatToAnchor.normalized, Vector3.up);
                        Quaternion newRotation = Quaternion.Slerp(boatRB.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
                        boatRB.MoveRotation(newRotation);
                    }
                }
            }
        }
        else
        {
            boatRB.AddForce(-boatRB.velocity * pullForce, ForceMode.Force);
        }
        lastWheelAngleX = currentAngleX;
        UpdateRopeWhileDeployed();

        if (anchor != null && boat != null && !moveComplete)
        {
            float distance = Vector3.Distance(boat.position, anchor.position);
            if (distance <= completeDistance)
            {
                moveComplete = true;
                StartCoroutine(OnMoveComplete());
            }
        }
    
    }
    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private IEnumerator OnMoveComplete()
    {
        while (boatRB.velocity.magnitude > 0.1f)
        {
            boatRB.AddForce(-boatRB.velocity * pullForce, ForceMode.Force);
            yield return null;
        }
        boatRB.velocity = Vector3.zero;
        moveCompleteEvent.RaiseEvent(null, this);
    }

    private void UpdateRopeWhileDeployed()
    {
        if (!ropeDeployed || anchor == null || lineRenderer == null) return;

        Vector3 start = shootPoint.position;
        Vector3 end = anchor.position;
        for (int i = 0; i < ropeSegments; i++)
        {
            float segmentT = (float)i / (ropeSegments - 1);
            Vector3 pos = Vector3.Lerp(start, end, segmentT);
            lineRenderer.SetPosition(i, pos);
        }
    }


    public void ShootRope(GameObject currentAnchor)
    {
        if (currentAnchor != null && lineRenderer != null)
        {
            moveComplete = false;
            anchor = currentAnchor.transform;
            Vector3 start = shootPoint.position;
            Vector3 end = currentAnchor.transform.position;
            lineRenderer.enabled = true;
            lineRenderer.positionCount = ropeSegments;

            // Start with all points at the start
            for (int i = 0; i < ropeSegments; i++)
                lineRenderer.SetPosition(i, start);

            // Animate the rope tip
            DOTween.To(
                () => 0f,
                t => UpdateRopeLine(start, end, t),
                1f,
                0.5f
            ).OnStart(() =>
            {
                if (wobbleCoroutine != null) StopCoroutine(wobbleCoroutine);
                wobbleCoroutine = StartCoroutine(WobbleRope(start, end));
            })
            .OnComplete(() =>
            {
                if (wobbleCoroutine != null) StopCoroutine(wobbleCoroutine);
                ropeDeployed = true;
            });
        }
    }
    private void UpdateRopeLine(Vector3 start, Vector3 end, float t)
    {
        for (int i = 0; i < ropeSegments; i++)
        {
            float segmentT = (float)i / (ropeSegments - 1);
            Vector3 pos = Vector3.Lerp(start, end, Mathf.Min(segmentT, t));
            lineRenderer.SetPosition(i, pos);
        }
    }

    private IEnumerator WobbleRope(Vector3 start, Vector3 end)
    {
        float time = 0f;
        while (true)
        {
            for (int i = 1; i < ropeSegments - 1; i++)
            {
                float segmentT = (float)i / (ropeSegments - 1);
                Vector3 pos = Vector3.Lerp(start, end, segmentT);

                // Wobble offset perpendicular to rope direction
                Vector3 dir = (end - start).normalized;
                Vector3 up = Vector3.up;
                if (Vector3.Dot(dir, up) > 0.99f) up = Vector3.right; // Avoid parallel
                Vector3 side = Vector3.Cross(dir, up).normalized;

                float wobble = Mathf.Sin(time * wobbleFrequency + segmentT * Mathf.PI * 2) * wobbleMagnitude * (1 - Mathf.Abs(segmentT - 0.5f) * 2);
                pos += side * wobble;

                lineRenderer.SetPosition(i, pos);
            }
            time += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator SettleRopeToStraight(Vector3 start, Vector3 end, float duration)
    {
        Vector3[] initialPositions = new Vector3[ropeSegments];
        for (int i = 0; i < ropeSegments; i++)
            initialPositions[i] = lineRenderer.GetPosition(i);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            for (int i = 0; i < ropeSegments; i++)
            {
                float segmentT = (float)i / (ropeSegments - 1);
                Vector3 straightPos = Vector3.Lerp(start, end, segmentT);
                Vector3 pos = Vector3.Lerp(initialPositions[i], straightPos, t);
                lineRenderer.SetPosition(i, pos);
            }
            yield return null;
        }
        // Ensure final positions are perfectly straight
        for (int i = 0; i < ropeSegments; i++)
        {
            float segmentT = (float)i / (ropeSegments - 1);
            lineRenderer.SetPosition(i, Vector3.Lerp(start, end, segmentT));
        }
        
    }


}
