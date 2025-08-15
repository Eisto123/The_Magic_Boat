using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    
    public OVRPassthroughLayer passthroughLayer;
    private AssetReference currentScene;
    public List<AssetReference> Maps;
    public AssetReference ARMode;
    public List<LevelTeleportPosSO> LevelTeleportPositions;
    public GameObject Boat;
    public Transform PlayerSittingPosition;
    public Transform OVRrig;
    public static int mapIndex = 0;
    public int teleportIndex = 0;

    void Start()
    {
        LoadScene(ARMode);
        currentScene = ARMode;
    }
    public void ToggleScene()
    {
        UnloadScene();
        if (currentScene == ARMode)
        {
            LoadScene(Maps[mapIndex]);
        }
        else
        {
            LoadScene(ARMode);
        }

    }

    private void LoadScene(AssetReference scene)
    {
        currentScene = scene;
        if (currentScene == ARMode)
        {
            StartCoroutine(LoadARSceneProcess());
        }
        else
        {
            StartCoroutine(LoadVRSceneProcess());
        }
    }
    IEnumerator LoadVRSceneProcess()
    {
        var s = currentScene.LoadSceneAsync(LoadSceneMode.Additive);
        yield return new WaitUntil(() => s.IsDone);
        
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        Camera.main.backgroundColor = Color.white;
        SceneManager.SetActiveScene(s.Result.Scene);
        Boat.SetActive(true);
        
        OVRrig.parent = PlayerSittingPosition;
        SetBoatPositionBaseOnIndex(mapIndex, teleportIndex);
        passthroughLayer.textureOpacity = 0;
    }
    IEnumerator LoadARSceneProcess()
    {
        var s = currentScene.LoadSceneAsync(LoadSceneMode.Additive);
        yield return new WaitUntil(() => s.IsDone);
        SceneManager.SetActiveScene(s.Result.Scene);
    }


    private void SetBoatPositionBaseOnIndex(int mapIndex, int TeleportIndex)
    {

        var teleportPositions = LevelTeleportPositions[mapIndex].teleportPositions;

        var targetPos = teleportPositions[TeleportIndex].targetPos;
        var targetRot = teleportPositions[TeleportIndex].targetRotation;

        Boat.transform.position = targetPos.ToVector3();
        Boat.transform.rotation = Quaternion.Euler(targetRot.ToVector3());
    }


    private void UnloadScene()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        currentScene.ReleaseAsset();
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = Color.clear;

        Boat.transform.position = Vector3.zero;
        Boat.transform.rotation = Quaternion.identity;
        Boat.SetActive(false);
        OVRrig.parent = null;
        passthroughLayer.textureOpacity = 1;
    }


    public void SetMapIndex(object obj)
    {
        int index = (int)obj;
        if (index >= 0 && index < Maps.Count)
        {
            mapIndex = index;
        }
        else
        {
            Debug.LogWarning("Map index out of range.");
        }
    }
    
    public void SetTeleportIndex(object obj)
    {
        int index = (int)obj;
        if (index >= 0 && index < LevelTeleportPositions[mapIndex].teleportPositions.Count)
        {
            teleportIndex = index;
        }
        else
        {
            Debug.LogWarning("Teleport index out of range.");
        }
    }

}
