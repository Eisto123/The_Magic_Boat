
using DG.Tweening;
using UnityEngine;

public class MagicBook : MonoBehaviour
{
    void OnEnable()
    {
        transform.localScale = Vector3.one * 0.01f; // Reset scale to a small value
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
}
