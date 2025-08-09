
using DG.Tweening;
using UnityEngine;

public class MagicBook : MonoBehaviour
{
    [Header("Magic Book Data")]
    public MagicBookData magicBookData;

    [Header("Magic Book UI")]
    public MagicBookUI magicBookUI;


    void OnEnable()
    {
        
        transform.localScale = Vector3.one * 0.01f; // Reset scale to a small value
        SetUpBookUI(SceneLoadManager.mapIndex);
        transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            Debug.Log("Magic Book is now fully scaled up.");
        });
    }

    public void DisableBook()
    {
        transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
            Debug.Log("Magic Book is now fully scaled down and deactivated.");
        });
    }

    public void SetUpBookUI(int bookIndex)
    {
        if (bookIndex < 0 || bookIndex >= magicBookData.bookDatas.Count)
        {
            Debug.LogWarning("Invalid book index: " + bookIndex);
            return;
        }

        BookData bookData = magicBookData.bookDatas[bookIndex];
        magicBookUI.UpdateBookUI(bookData);
    }
}
