using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARSceneManager : MonoBehaviour
{
    public static ARSceneManager Instance;
    public GameObject ARModel;
    public GameObject TeleportBoat;


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
    }

    public void ShowARModel()
    {
        if (ARModel != null)
        {
            ARModel.SetActive(true);
        }
    }
    public void HideARModel()
    {
        if (ARModel != null)
        {
            ARModel.SetActive(false);
        }
    }
    public void ShowBoat()
    {
        if (TeleportBoat != null)
        {
            TeleportBoat.SetActive(true);
        }
    }
    public void HideBoat()
    {
        if (TeleportBoat != null)
        {
            TeleportBoat.SetActive(false);
        }
    }

    
}
