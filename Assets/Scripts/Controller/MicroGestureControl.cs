using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MicroGestureControl : MonoBehaviour
{
    public OVRHand rightHand;
    public Rigidbody boatRigidbody;
    private bool isMoving = false;
    public float moveSpeed = 10f;
    public float turnAngle = 30f;
    public float turnTorque = 100f;
    public float turnDuration = 1f;
    public float maxSpeed = 20f;
    public float breakForce = 10f;
    private bool isTurning = false;
    private float turnProgress = 0f;
    private Vector3 turnStartDirection;
    private Vector3 turnTargetDirection;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        OVRHand.MicrogestureType microgestureType = rightHand.GetMicrogestureType();

        switch (microgestureType)
        {
            case OVRHand.MicrogestureType.NoGesture:
                break;
            case OVRHand.MicrogestureType.ThumbTap:
                ToggleMoving();
                break;
            case OVRHand.MicrogestureType.SwipeLeft:
                Turn(true); // Turn left
                break;
            case OVRHand.MicrogestureType.SwipeRight:
                Turn(false); // Turn right
                break;
            default:
                break;
        }
    }
    void FixedUpdate()
    {
        Move();
        if (isTurning)
        {
            turnProgress += Time.fixedDeltaTime / turnDuration;
            if (turnProgress >= 1f)
            {
                turnProgress = 1f;
                isTurning = false;
            }
        }
        }
    private void ToggleMoving()
    {
        isMoving = !isMoving;
    }

    private void Move()
    {
        if (isMoving)
        {
            Vector3 forceDirection = transform.forward;
            if (isTurning)
            {
                // Blend between start and target direction
                forceDirection = Vector3.Slerp(turnStartDirection, turnTargetDirection, turnProgress);
            }
            boatRigidbody.AddForce(forceDirection.normalized * moveSpeed, ForceMode.Force);

            if (boatRigidbody.velocity.magnitude > maxSpeed)
            {
                boatRigidbody.velocity = boatRigidbody.velocity.normalized * maxSpeed;
            }
        }
        else
        {
            boatRigidbody.AddForce(-boatRigidbody.velocity * moveSpeed, ForceMode.Force);
        }
    }
    public void Turn(bool turnLeft)
    {
        if (!isTurning)
        {
            isTurning = true;
            turnProgress = 0f;
            turnStartDirection = transform.forward;
            // Calculate the target direction after turnAngle
            float angle = turnLeft ? -turnAngle : turnAngle;
            turnTargetDirection = Quaternion.Euler(0, angle, 0) * transform.forward;
        }
        // Apply torque to start turning
        float torque = turnLeft ? -turnTorque : turnTorque;
        boatRigidbody.AddTorque(Vector3.up * torque, ForceMode.Impulse);
    }

}
