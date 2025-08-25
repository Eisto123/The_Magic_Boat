using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float floatDistance = 1f;
    public float duration = 2f;       
    public float fadeInTime = 0.3f;   
    public float fadeOutTime = 0.5f;  
    public float stayTime = 1f;       

    private Transform mainCamera;

    private void Awake()
    {
        if (Camera.main != null)
            mainCamera = Camera.main.transform;
    }

    private void LateUpdate()
    {        
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.rotation * Vector3.forward,
                             mainCamera.rotation * Vector3.up);
        }
    }

    public void Show(string message, Color color)
    {
        text.text = message;
        text.color = new Color(color.r, color.g, color.b, 0); 

       
        transform.DOMoveY(transform.position.y + floatDistance, duration);

        
        text.DOFade(1, fadeInTime)
            .OnComplete(() =>
            {
                DOVirtual.DelayedCall(stayTime, () =>
                {
                    text.DOFade(0, fadeOutTime).OnComplete(() => Destroy(gameObject));
                });
            });
    }
}
