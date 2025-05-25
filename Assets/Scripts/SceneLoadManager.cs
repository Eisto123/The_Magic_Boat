using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public OVRPassthroughLayer passthroughLayer;
    private AssetReference currentScene;
    public List<AssetReference> Maps;
    public int mapIndex = 0;

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
        StartCoroutine(LoadSceneProcess());
    }
    IEnumerator LoadSceneProcess()
    {
        var s = currentScene.LoadSceneAsync(LoadSceneMode.Additive);
        yield return new WaitUntil(() => s.IsDone);
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        Camera.main.backgroundColor = Color.white;
        SceneManager.SetActiveScene(s.Result.Scene);
        
        passthroughLayer.enabled = false;

    }
    private void UnloadScene()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        currentScene.ReleaseAsset();
        currentScene = null;
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = Color.clear;
        passthroughLayer.enabled = true;

    }

}
