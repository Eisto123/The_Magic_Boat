using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DOTweenAnimationEnable : MonoBehaviour
{
    void OnEnable()
    {
        transform.DOScale(Vector3.one, 1f).From(Vector3.zero).SetEase(Ease.OutBack);
    }
}
