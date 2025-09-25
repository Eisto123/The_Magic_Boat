
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class MagicBook : MonoBehaviour
{
    [Header("Magic Book Data")]
    public MagicBookData magicBookData;

    [Header("Magic Book UI")]
    public MagicBookUI magicBookUI;
    private int currentBookIndex;
    public List<PlayableDirector> pageturnDirectors;

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
        magicBookUI.FadeUI(false, 1f);
        if (bookIndex < 0 || bookIndex >= magicBookData.bookDatas.Count)
        {
            Debug.LogWarning("Invalid book index: " + bookIndex);
            return;
        }

        int pageDelta = bookIndex - currentBookIndex;
        if (pageDelta != 0)
        {
            int absDelta = Mathf.Abs(pageDelta);
            absDelta = Mathf.Clamp(absDelta, 1, 3);

            PlayableDirector director = null;
            if (pageDelta > 0)
                director = pageturnDirectors[absDelta - 1]; // forward
            else
                director = pageturnDirectors[absDelta + 2]; // backward

            director.stopped += OnDirectorStopped;
            director.Play();
            UpdateBookUIAfterTurn(bookIndex);
        }
        else
        {
            UpdateBookUIAfterTurn(bookIndex);
        }
    }


    private void OnDirectorStopped(PlayableDirector director)
    {
        director.stopped -= OnDirectorStopped;
        UpdateBookUIAfterTurn(currentBookIndex);
    }


    private void UpdateBookUIAfterTurn(int bookIndex)
    {
        magicBookUI.FadeUI(true, 1f);
        BookData bookData = magicBookData.bookDatas[bookIndex];
        magicBookUI.UpdateBookUI(bookData);
        currentBookIndex = bookIndex;
    }

}
