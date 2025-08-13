using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    [SerializeField]
    List<Popup> popups = new List<Popup>();


    public T ShowPopup<T>() where T : Popup
    {
        var popup = popups.Find(p => p is T);
        if (popup == null)
        {
            Debug.LogError($"Popup of type {typeof(T)} not found.");
            return null;
        }
        popup.Show();
        return popup as T;
    }

    public T HidePopup<T>() where T : Popup
    {
        var popup = popups.Find(p => p is T);
        if (popup == null)
        {
            Debug.LogError($"Popup of type {typeof(T)} not found.");
            return null;
        }
        popup.Hide();
        return popup as T;
    }
}
