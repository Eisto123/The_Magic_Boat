using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    [SerializeField] private float rotateSpeed = 2f;
    public bool moveComplete = false;
    public ObjectEventSO moveCompleteEvent;

    private Coroutine wobbleCoroutine;
    private Rigidbody boatRB;

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
        if (!ropeDeployed) return;

        float currentAngleX = wheel.localEulerAngles.x;
        float deltaAngle = Mathf.DeltaAngle(lastWheelAngleX, currentAngleX);

        // Anti-clockwise (negative delta): pull boat closer
        if (deltaAngle < 0 && anchor != null)
        {
            if (boatRB != null)
            {
                Vector3 direction = (anchor.position - boat.position).normalized;
                float force = Mathf.Abs(deltaAngle) * pullForce;
                boatRB.AddForce(direction * force, ForceMode.Force);
                if (direction.sqrMagnitude > 0.001f)
                {
                    Quaternion ReelRotation = Quaternion.LookRotation(direction, Vector3.up);
                    float dynamicRotateSpeed = Mathf.Abs(deltaAngle) * rotateSpeed;
                    this.transform.rotation = Quaternion.Slerp(transform.rotation, ReelRotation, dynamicRotateSpeed * Time.fixedDeltaTime);
                    Vector3 flatDirection = new Vector3(direction.x, 0, direction.z).normalized;
                    if (flatDirection.sqrMagnitude > 0.001f)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(flatDirection, Vector3.up);
                        boat.rotation = Quaternion.Slerp(boat.rotation, targetRotation, dynamicRotateSpeed * Time.fixedDeltaTime);
                        
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
    private IEnumerator OnMoveComplete()
    {
        while (boatRB.velocity.magnitude > 0.1f)
        {
            boatRB.AddForce(-boatRB.velocity * pullForce, ForceMode.Force);
            yield return null;
        }
        boatRB.velocity = Vector3.zero;
        moveCompleteEvent.RaiseEvent(null,this);
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
                StartCoroutine(SettleRopeToStraight(start, end, 2f));
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
        ropeDeployed = true;
    }


}
