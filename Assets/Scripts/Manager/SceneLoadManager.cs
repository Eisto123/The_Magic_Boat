using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager instance;
    public static bool FadeInIsDone = false;
    public OVRPassthroughLayer passthroughLayer;
    private AssetReference currentScene;
    public List<AssetReference> Maps;
    public AssetReference ARMode;
    public List<LevelTeleportPosSO> LevelTeleportPositions;
    public GameObject Boat;
    public CameraRigManager OVRrig;

    [Header("FadeMask")]
    public GameObject WhiteFadeMask;
    public GameObject DepthMask;
    public Camera MainCamera;
    private float originalFarClipPlane;
    private int _AphaForDepthMask = Shader.PropertyToID("_Alpha");
    public static int mapIndex = 0;
    public int teleportIndex = 0;
    private SceneLoader currentSceneLoader;
    public bool skipTutorial = false;
    public StringEventSO TutorialCompleteEvent;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        StartCoroutine(StartHoldingTime());

    }

    private IEnumerator StartHoldingTime()
    {
        if (!skipTutorial)
            yield return new WaitForSeconds(15f);

        else
            yield return null;
        LoadScene(ARMode);
        currentScene = ARMode;
        TutorialCompleteEvent.RaiseEvent("Welcome",this);
    }
    public void ToggleScene()
    {

        UnloadScene();
        if (currentScene == ARMode)
        {
            FadeInMask();
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

        Scene loadedScene = s.Result.Scene;
        SceneManager.SetActiveScene(loadedScene);

        while (!FadeInIsDone)
        {
            yield return null; // Wait until fade-in is complete
        }
        MainCamera.clearFlags = CameraClearFlags.Skybox;
        MainCamera.backgroundColor = Color.white;
        Boat.SetActive(true);
        OVRrig.gameObject.transform.SetParent(Boat.transform);
        OVRrig.transform.localRotation = Quaternion.identity;
        //OVRrig.SetFollowBoat(true);
        SetBoatPositionBaseOnIndex(mapIndex, teleportIndex);
        passthroughLayer.textureOpacity = 0;

        BGMManager.Instance.PlayBGM(0);
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
        List<Scene> scenesToUnload = new List<Scene>();
        int sceneCount = SceneManager.sceneCount;
        for (int i = 0; i < sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != "Persistant")
            {
                scenesToUnload.Add(scene);
            }
        }

        foreach (var scene in scenesToUnload)
        {
            SceneManager.UnloadSceneAsync(scene);
        }

        MainCamera.clearFlags = CameraClearFlags.SolidColor;
        MainCamera.backgroundColor = Color.clear;

        Boat.transform.position = Vector3.zero;
        Boat.transform.eulerAngles = Vector3.zero;
        Boat.GetComponent<Rigidbody>().velocity = Vector3.zero;
        Boat.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        Boat.GetComponent<MicroGestureControl>().isMoving = false;
        Boat.SetActive(false);
        OVRrig.gameObject.transform.SetParent(null);
        //OVRrig.SetFollowBoat(false);
        passthroughLayer.textureOpacity = 1;
        BGMManager.Instance.FadeOutBGM();
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

    #region Fade Mask

    public void FadeInMask()
    {
        WhiteFadeMask.SetActive(true);
        WhiteFadeMask.GetComponent<MeshRenderer>().material.DOColor(Color.white, 1f).OnComplete(() =>
        {
            FadeInIsDone = true;
        });
        originalFarClipPlane = MainCamera.farClipPlane;

        DOTween.To(
            () => MainCamera.farClipPlane,
            x => MainCamera.farClipPlane = x,
            1f,
            1f
        );
    }

    public async Task FadeOutMask()
    {
        while (!FadeInIsDone)
        {
            await Task.Delay(100);
        }
        DepthMask.SetActive(true);
        var material = DepthMask.GetComponent<MeshRenderer>().material;
        material.SetFloat(_AphaForDepthMask, 1f);

        WhiteFadeMask.GetComponent<MeshRenderer>().material.color = Color.clear;
        DOTween.To(
            () => MainCamera.farClipPlane,
            x => MainCamera.farClipPlane = x,
            originalFarClipPlane,
            2f
        ).OnComplete(() =>
        {
            material.DOFloat(0f, _AphaForDepthMask, 0.5f).OnComplete(() =>
            {
                DepthMask.SetActive(false);
                FadeInIsDone = false;
            });
            
        });
    }
    
    #endregion

}
