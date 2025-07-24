using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Wire : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Image image;
    private CableMiniGame cableMiniGame;
    private Transform parentAfterDrag;

    private void Start()
    {
        image = GetComponent<Image>();
        cableMiniGame = FindObjectOfType<CableMiniGame>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        cableMiniGame.DisableRaycasts();
        transform.SetParent(cableMiniGame.transform);
        parentAfterDrag = transform.parent;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        cableMiniGame.EnableRaycasts();
        transform.SetParent(parentAfterDrag);
    }

    internal WireSlot GetParentSlot() => transform.parent.GetComponent<WireSlot>();

    internal void DisableRaycast() => image.raycastTarget = false;

    internal void EnableRaycast() => image.raycastTarget = true;
}
