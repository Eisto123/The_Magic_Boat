using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneGroupManager
{
    public event Action<String> OnSceneLoaded = delegate { };
    public event Action<String> OnSceneUnloaded = delegate { };
    public Action OnSceneGroupLoaded = delegate { };

    public SceneGroup CurrentSceneGroup { get; private set; }
    public AsyncOperationHandleGroup CurrentSceneHandles { get; private set; } = new AsyncOperationHandleGroup(10);

    public async Task LoadScene(SceneData sceneData, IProgress<float> progress)
    {
        var handle = sceneData.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
        CurrentSceneHandles.Handles.Add(handle);
        while (!handle.IsDone)
        {
            progress.Report(handle.PercentComplete);
            await Task.Delay(100);
        }
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            OnSceneLoaded?.Invoke(sceneData.sceneName);
        }
    }
    public async Task UnloadScene(SceneData sceneData, IProgress<float> progress)
    {
        var handle = sceneData.sceneReference.UnLoadScene();
        CurrentSceneHandles.Handles.Remove(handle);
        while (!handle.IsDone)
        {
            progress.Report(handle.PercentComplete);
            await Task.Delay(100);
        }
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            OnSceneUnloaded?.Invoke(sceneData.sceneName);
        }
    }
}


public readonly struct AsyncOperationHandleGroup
{
    public readonly List<AsyncOperationHandle<SceneInstance>> Handles;
    public float Progress => Handles.Count == 0 ? 0 : Handles.Average(h => h.PercentComplete);
    public bool IsDone => Handles.Count == 0 || Handles.All(o => o.IsDone);
    public AsyncOperationHandleGroup(int initialCapacity)
    {
        Handles = new List<AsyncOperationHandle<SceneInstance>>(initialCapacity);
    }
}


