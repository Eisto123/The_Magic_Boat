using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class SceneLoader : MonoBehaviour
{
    public SceneGroup sceneGroup;
    public ObjectEventSO FadeInMaskCompleteEvent;
    private bool fadeInisDone = false;
    public List<AsyncOperationHandle<SceneInstance>> pendingHandles = new List<AsyncOperationHandle<SceneInstance>>();

    async void Start()
    {
        await LoadAllScenesHoldActivation();
    }
    void OnEnable()
    {
        FadeInMaskCompleteEvent.onEventRaised += SetFadeInComplete;
    }
    void OnDisable()
    {
        FadeInMaskCompleteEvent.onEventRaised -= SetFadeInComplete;
    }

    private void SetFadeInComplete(object signal)
    {
        fadeInisDone = true;
    }

    public async Task LoadAllScenesHoldActivation()
    {
        var sceneGroupManager = new SceneGroupManager();
        var progress = new LoadingProgress();

        pendingHandles.Clear();

        // Load all scenes with activateOnLoad: false
        foreach (var sceneData in sceneGroup.scenes)
        {
            var handle = await sceneGroupManager.LoadScene(sceneData, progress, false);
            pendingHandles.Add(handle);
        }

        while (!fadeInisDone)
        {
            await Task.Delay(100); // Wait until fade-in is complete
        }

        ActivateAllLoadedScenes();
        await SceneLoadManager.instance.FadeOutMask();
    }
    public async Task LoadScene()
    {
        var sceneGroupManager = new SceneGroupManager();
        var progress = new LoadingProgress();
        pendingHandles.Clear();
        foreach (var sceneData in sceneGroup.scenes)
        {
            var handle = await sceneGroupManager.LoadScene(sceneData, progress, false);
            pendingHandles.Add(handle);
        }
    }
    public void ActivateAllLoadedScenes()
    {
        foreach (var handle in pendingHandles)
        {
            if (handle.IsValid() && !handle.Result.Scene.isLoaded)
            {
                handle.Result.ActivateAsync();
            }
        }
    }
}

public class LoadingProgress : IProgress<float>
{
    public event Action<float> Progressed;

    const float ratio = 1f;

    public void Report(float value)
    {
        Progressed?.Invoke(value / ratio);
    }
}
