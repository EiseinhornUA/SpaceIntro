using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InteractionView : MonoBehaviour
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

    internal void Hide() => gameObject.SetActive(false);
    internal void Show() => gameObject.SetActive(true);
}
