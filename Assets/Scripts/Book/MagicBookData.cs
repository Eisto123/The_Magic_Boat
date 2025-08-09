using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BookData
{
    public string levelName;
    public string levelDescription;
    public Sprite levelImage;
    public List<ElementCollectable> collectables;

}

[System.Serializable]
public class ElementCollectable
{
    public string name;
    public Sprite image;
    public Element elementType;
    public bool isCollected = false;
}



[CreateAssetMenu(fileName = "MagicBookData", menuName = "ScriptableObjects/MagicBookData", order = 1)]
public class MagicBookData : ScriptableObject
{
    public List<BookData> bookDatas;
}
