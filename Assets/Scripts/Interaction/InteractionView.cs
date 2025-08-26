using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractionView : Popup
{
    [SerializeField] private Button interactionButton;

    internal void AddListener(UnityAction onInteract)
    {
        interactionButton.onClick.AddListener(onInteract);
    }
    internal void RemoveListener(UnityAction onInteract)
    {
        interactionButton.onClick.RemoveListener(onInteract);
    }

    public void SetPosition(Vector3 position)
    {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(position);
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.position = screenPosition;
    }
}
