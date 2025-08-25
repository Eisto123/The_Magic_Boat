using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;
    public FloatingText floatingTextPrefab; // 拖到 Inspector

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowFloatingMessage(string message, Vector3 worldPosition, Color color)
    {
        if (floatingTextPrefab != null)
        {
            FloatingText ft = Instantiate(floatingTextPrefab, worldPosition, Quaternion.identity);
            ft.Show(message, color);
        }
    }
}
