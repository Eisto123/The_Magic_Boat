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
    public List<LevelTeleportPosSO> LevelTeleportPositions;
    public GameObject Boat;
    public Transform PlayerSittingPosition;
    public Transform OVRrig;
    public int mapIndex = 0;
    public int teleportIndex = 0;
    public GameObject FadeMask;

    public void ToggleScene()
    {
        if (currentScene == null)
        {
            LoadScene(Maps[mapIndex]);
        }
        else
        {
            UnloadScene();
        }

    }

    private void LoadScene(AssetReference scene)
    {
        
        currentScene = scene;
        FadeMask.GetComponent<MeshRenderer>().material.DOFloat(0.6f, "_CutoffHeight", 0.5f).SetEase(Ease.InOutSine);
        FadeMask.GetComponent<MeshRenderer>().material.DOFloat(1, "_Alpha", 0.5f).SetEase(Ease.InOutSine);
        StartCoroutine(LoadSceneProcess());
    }
    IEnumerator LoadSceneProcess()
    {
        var s = currentScene.LoadSceneAsync(LoadSceneMode.Additive);
        yield return new WaitUntil(() => s.IsDone);
        FadeMask.transform.DOScale(new Vector3(100, 100, 100), 5f).SetEase(Ease.InOutSine);
        FadeMask.GetComponent<MeshRenderer>().material.DOFloat(0, "_Alpha", 5f).SetEase(Ease.InOutSine).onComplete = () =>
        {
            FadeMask.transform.localScale = Vector3.one;
            FadeMask.GetComponent<MeshRenderer>().material.SetFloat("_CutoffHeight", -0.6f);
        };
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        Camera.main.backgroundColor = Color.white;
        SceneManager.SetActiveScene(s.Result.Scene);
        Boat.SetActive(true);

        OVRrig.parent = PlayerSittingPosition;
        SetBoatPositionBaseOnIndex(mapIndex, teleportIndex);
        passthroughLayer.enabled = false;
        

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
        currentScene = null;
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = Color.clear;

        Boat.transform.position = Vector3.zero;
        Boat.transform.rotation = Quaternion.identity;
        Boat.SetActive(false);
        OVRrig.parent = null;
        passthroughLayer.enabled = true;
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
