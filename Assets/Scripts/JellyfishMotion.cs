using UnityEngine;

public class JellyfishMotion : MonoBehaviour
{
    [Header("Floating")]
    public float floatAmplitude = 0.2f;
    public float floatFrequency = 1f;

    [Header("Wobble")]
    public float wobbleAmplitude = 10f;   // degrees
    public float wobbleFrequency = 2f;

    [Header("Rotation")]
    public float rotationSpeed = 10f;     // deg/sec, around Y

    [Header("Desync (per-instance randomization)")]
    public bool randomizeOnStart = true;
    public Vector2 floatAmpRange = new Vector2(0.15f, 0.30f);
    public Vector2 floatFreqRange = new Vector2(0.80f, 1.30f);
    public Vector2 wobbleAmpRange = new Vector2(8f, 14f);
    public Vector2 wobbleFreqRange = new Vector2(1.2f, 2.5f);
    public Vector2 rotSpeedRange = new Vector2(5f, 20f);

    // Per-instance phase offsets
    private float floatPhase;
    private float wobblePhase;
    private float rotPhase;

    private Vector3 startPos;
    private Quaternion startRot;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;

        if (randomizeOnStart)
        {
            floatAmplitude = Random.Range(floatAmpRange.x, floatAmpRange.y);
            floatFrequency = Random.Range(floatFreqRange.x, floatFreqRange.y);
            wobbleAmplitude = Random.Range(wobbleAmpRange.x, wobbleAmpRange.y);
            wobbleFrequency = Random.Range(wobbleFreqRange.x, wobbleFreqRange.y);
            rotationSpeed = Random.Range(rotSpeedRange.x, rotSpeedRange.y);
        }

        // Random phase so每只水母起步时间不同
        floatPhase = Random.Range(0f, Mathf.PI * 2f);
        wobblePhase = Random.Range(0f, Mathf.PI * 2f);
        rotPhase = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        float t = Time.time;

        // 上下浮动（加入相位）
        float newY = startPos.y + Mathf.Sin(t * floatFrequency + floatPhase) * floatAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // 左右扭动（Z轴），加入相位
        float wobbleZ = Mathf.Sin(t * wobbleFrequency + wobblePhase) * wobbleAmplitude;
        Quaternion wobbleRotation = Quaternion.Euler(0f, 0f, wobbleZ);

        
    }
}
