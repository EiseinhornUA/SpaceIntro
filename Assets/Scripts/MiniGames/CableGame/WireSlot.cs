using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WireSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            Wire wire = eventData.pointerDrag.GetComponent<Wire>();
            wire.SetParentAfterDrag(transform);
        }
    }

    internal Wire GetWire()
    {
        return transform.GetComponentInChildren<Wire>();
    }
}
