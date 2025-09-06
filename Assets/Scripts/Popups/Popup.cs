using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    [ContextMenu("Show")]
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    [ContextMenu("Hide")]
    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}