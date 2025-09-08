using UnityEngine;
using UnityEngine.UI;

public class PopupView : MonoBehaviour
{
    [SerializeField] private Button closeButton;

    private void Start()
    {
        if (closeButton) closeButton.onClick.AddListener(Hide);
    }

    [ContextMenu("Show")]
    public void Show()
    {
        gameObject.SetActive(true);
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}