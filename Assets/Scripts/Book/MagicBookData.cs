using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BookData
{
    public string levelName;
    public string levelDescription;
    public Sprite levelImage;
    public string item;
    public string buffItems;
}

[CreateAssetMenu(fileName = "MagicBookData", menuName = "ScriptableObjects/MagicBookData", order = 1)]
public class MagicBookDataSO : ScriptableObject
{
    public BookData bookData;
}
