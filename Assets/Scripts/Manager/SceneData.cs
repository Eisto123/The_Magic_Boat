using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[System.Serializable]
public class SceneData
{
    public AssetReference sceneReference;
    public string sceneName;
    public SceneType sceneType;
}

[System.Serializable]
public class SceneGroup
{
    public string groupName;
    public List<SceneData> scenes;

    public SceneGroup(string name)
    {
        groupName = name;
        scenes = new List<SceneData>();
    }
}
