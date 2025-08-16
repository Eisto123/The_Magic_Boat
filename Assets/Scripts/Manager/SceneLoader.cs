using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public SceneGroup sceneGroup;

    async void Start()
    {
        await LoadScene();
    }
    public async Task LoadScene()
    {
        var sceneGroupManager = new SceneGroupManager();
        var progress = new LoadingProgress();

        for (int i = 0; i < sceneGroup.scenes.Count; i++)
        {
            var sceneData = sceneGroup.scenes[i];
            progress.Progressed += value => Debug.Log($"Loading {sceneData.sceneName}: {value * 100}%");
            await sceneGroupManager.LoadScene(sceneData, progress);
        }
    }
}

public class LoadingProgress : IProgress<float> {
        public event Action<float> Progressed;

        const float ratio = 1f;

        public void Report(float value) {
            Progressed?.Invoke(value / ratio);
        }
}
