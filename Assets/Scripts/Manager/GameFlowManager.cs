using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    [Header("Magic Book Data")]
    public MagicBookData magicBookData;

    void Start()
    {
        ResetCollectables();
    }

    public void OnItemCollect(object collectableData)
    {
        CollectableData data = (CollectableData)collectableData;
        Element element = data.elementType;
        for (int i = 0; i < magicBookData.bookDatas.Count; i++)
        {
            foreach (ElementCollectable collectable in magicBookData.bookDatas[i].collectables)
            {
                if (collectable.elementType == element && !collectable.isCollected)
                {
                    collectable.isCollected = true;
                    Debug.Log($"Collected: {collectable.name}");
                    break;
                }
            }
        }

    }
    
    public void ResetCollectables()
    {
        foreach (var bookData in magicBookData.bookDatas)
        {
            foreach (var collectable in bookData.collectables)
            {
                collectable.isCollected = false;
            }
        }
        Debug.Log("All collectables have been reset.");
    }
}
