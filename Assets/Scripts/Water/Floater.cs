using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    private Rigidbody rb;
    public Transform[] floaters;
    public float waterDrag = 1;
    public float waterAngularDrag = 1f;
    public float airDrag = 0f;
    public float airAngularDrag = 0.05f;
    public float floatingPower = 15f;
    bool underWater = false;
    int floaterBelowWater = 0;

    void OnEnable()
    {
        if(transform.parent == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        else
        {
            rb = GetComponentInParent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {
        if(WaterManager.Instance == null)
        {
            return;
        }
        floaterBelowWater = 0;
        for (int i = 0; i < floaters.Length; i++)
        {
            float diffence = floaters[i].position.y - WaterManager.Instance.GetWaveYAtPosition(floaters[i].position);
            if (diffence < 0)
            {
                rb.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(diffence), floaters[i].position, ForceMode.Force);
                floaterBelowWater++;
                if (!underWater)
                {
                    underWater = true;
                    rb.drag = waterDrag;
                    rb.angularDrag = waterAngularDrag;
                }
            }
        }
        if (floaterBelowWater == 0 && underWater)
        {
            underWater = false;
            rb.drag = airDrag;
            rb.angularDrag = airAngularDrag;
        }
        
    }
}
