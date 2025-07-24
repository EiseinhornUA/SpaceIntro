using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WireSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (!eventData.pointerDrag.TryGetComponent<Wire>(out Wire draggedWire)) return;

        if (IsEmpty())
        {
            draggedWire.transform.SetParent(transform);
            return;
        }
        Wire existingWire = GetWire();
        Transform previousParent = draggedWire.transform.parent;

        existingWire.transform.SetParent(previousParent);

        draggedWire.transform.SetParent(transform);
    }

    private bool IsEmpty()
    {
        return transform.childCount == 0;
    }

    internal Wire GetWire()
    {
        return transform.GetComponentInChildren<Wire>();
    }
}
