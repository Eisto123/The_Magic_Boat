using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Collectable : MonoBehaviour
{
    public bool isCollected = false;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }
    // Update is called once per frame
    void Update()
    {
        if (isCollected)
        {
            meshRenderer.material.color = Color.green; // Change color to indicate collection
        }
        else
        {
            meshRenderer.material.color = Color.white; // Reset color if not collected
        }
    }
}
