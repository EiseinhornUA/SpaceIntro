using System;
using UnityEngine;

public class EngineeringDoor : MonoBehaviour
{
    public Action OnDoorDestroyed;

    internal void Open()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        OnDoorDestroyed?.Invoke();
    }
}

