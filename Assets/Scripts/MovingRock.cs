using UnityEngine;

public class MovingRock : MonoBehaviour
{
    [Header("Floating Settings")]
    public float amplitude = 0.2f; 
    public float frequency = 1f;  

    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.up; 
    public float rotationSpeed = 10f;        

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {       
        float newY = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

       


        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}
