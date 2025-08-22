using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnchorState
{
    NotDetected,
    Unselected,
    Selected,
}
public class Anchor : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    public AnchorState anchorState = AnchorState.NotDetected;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void SetAnchorState(AnchorState state)
    {
        anchorState = state;
    }

    // Update is called once per frame
    void Update()
    {
        switch (anchorState)
        {
            case AnchorState.NotDetected:
                meshRenderer.material.color = Color.gray;
                break;
            case AnchorState.Unselected:
                meshRenderer.material.color = Color.yellow;
                break;
            case AnchorState.Selected:
                meshRenderer.material.color = Color.green;
                break;
            default:
                meshRenderer.material.color = Color.white;
                break;
        }
    }
}
