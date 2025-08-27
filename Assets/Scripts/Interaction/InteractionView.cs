using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractionView : Popup
{
    [SerializeField] private Button interactionButton;
    private Vector3 worldPosition;

    private void Update()
    {
        UpdatePosition();
    }

    internal void AddListener(UnityAction onInteract)
    {
        interactionButton.onClick.AddListener(onInteract);
    }
    internal void RemoveListener(UnityAction onInteract)
    {
        interactionButton.onClick.RemoveListener(onInteract);
    }

    public void SetPosition(Vector3 position) => this.worldPosition = position;

    public void UpdatePosition()
    {
        GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(worldPosition);
    }
}
