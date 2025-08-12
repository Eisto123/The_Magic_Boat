using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public struct CollectableData
{
    public Vector3 position;
    public Element elementType;
}

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class Collectable : MonoBehaviour
{
    public bool isCollected = false;
    public bool duringCollect = false; // Used to check if the collectable is currently being collected
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    public Element elementType;
    public float collectTime = 2f;

    private Coroutine collectCoroutine;
    public GameObject UICanvas;
    public Slider collectSlider; // Assign in inspector
    public ObjectEventSO OnItemCollectEvent;

    public ObjectEventSO OnStartCollectEvent;
    public ObjectEventSO OnStopCollectEvent;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        if (collectSlider != null)
            collectSlider.value = 0f;
    }

    void Start()
    {
        if (UICanvas != null)
            UICanvas.SetActive(false);
        switch (elementType)
        {
            case Element.Fire:
                meshRenderer.material.color = Color.red;
                break;
            case Element.Water:
                meshRenderer.material.color = Color.blue;
                break;
            case Element.Earth:
                meshRenderer.material.color = Color.yellow;
                break;
            case Element.Metal:
                meshRenderer.material.color = Color.white;
                break;
            case Element.Wood:
                meshRenderer.material.color = Color.green;
                break;
            default:
                meshRenderer.material.color = Color.gray; // Default color
                break;
        }
    }
    void Update()
    {
    }

    public void StartCollect()
    {
        if (collectCoroutine == null && !isCollected)
        {
            UICanvas.SetActive(true);
            duringCollect = true;
            collectCoroutine = StartCoroutine(CollectRoutine());
            OnStartCollectEvent.RaiseEvent(transform.position,this);
        }
    }

    public void StopCollect()
    {
        if (collectCoroutine != null)
        {
            StopCoroutine(collectCoroutine);
            collectCoroutine = null;
        }
        isCollected = false;
        if (collectSlider != null)
            collectSlider.value = 0f;
        UICanvas.SetActive(false);
        duringCollect = false;
        OnStopCollectEvent.RaiseEvent(null,this);
    }

    private IEnumerator CollectRoutine()
    {
        float timer = 0f;
        if (collectSlider != null)
        {
            collectSlider.maxValue = collectTime;
            collectSlider.value = 0f;
        }
        while (timer < collectTime)
        {
            timer += Time.deltaTime;
            if (collectSlider != null)
                collectSlider.value = timer;
            yield return null;
        }
        isCollected = true;
        collectCoroutine = null;
        if (collectSlider != null)
            collectSlider.value = collectTime;
        Debug.Log("Collectable collected: " + gameObject.name);
        UICanvas.SetActive(false);
        OnItemCollectEvent?.RaiseEvent(new CollectableData { position = transform.position, elementType = elementType},this);
        OnStopCollectEvent.RaiseEvent(null,this);
        transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutElastic).OnComplete(() => Destroy(gameObject));
    }
}
