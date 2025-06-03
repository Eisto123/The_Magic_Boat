using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    public static WaterManager Instance;
    public Material waterMaterial;
    private float waveFrequency = 1f;
    private float waveSpeed = 1f;
    private float waveAmplitude = 1f;
    //public MeshFilter meshFilter;
    public Transform waveTransform; // Assign the wave plane's transform in the Inspector


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //meshFilter = GetComponent<MeshFilter>();
    }

    // Start is called before the first frame update
    void Start()
    {
        waveFrequency = waterMaterial.GetFloat("_WaveFrequency");
        waveSpeed = waterMaterial.GetFloat("_WaveSpeed");
        waveAmplitude = waterMaterial.GetFloat("_WaveAmplitude");
    }
    public float GetWaveYAtPosition(Vector3 worldPos)
    {
        float x = worldPos.x / waveTransform.lossyScale.x;
        float t = Time.time;

        // Apply formula: y' = y - A * sin(x * f - t * s)
        float offset = waveAmplitude * Mathf.Sin(x * waveFrequency - t * waveSpeed);
        return transform.position.y - offset;
    }

    // void Update()
    // {
    //     Vector3[] vertices = meshFilter.mesh.vertices;
    //     for (int i = 0; i < vertices.Length; i++)
    //     {
    //         Vector3 worldPos = transform.TransformPoint(vertices[i]);
    //         vertices[i].y = GetWaveYAtPosition(worldPos);
    //     }
    //     meshFilter.mesh.vertices = vertices;
    //     meshFilter.mesh.RecalculateNormals();
    // }
}
