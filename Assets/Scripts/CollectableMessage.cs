using UnityEngine;

public class CollectableMessage : MonoBehaviour
{
    public string customMessage = "111";
    public Color textColor = Color.white;

    private void OnDestroy()
    {
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowFloatingMessage(customMessage, transform.position, textColor);
        }
    }
}
