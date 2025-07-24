using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CableMiniGame : MonoBehaviour
{
    private const int size = 6;
    [SerializeField] private List<WireSlot> wireSlots;

    private Wire GetWire(Vector2Int position)
    {
        return wireSlots[position.x + (position.y * size)].GetWire();
    }

    internal void DisableRaycasts()
    {
        foreach (WireSlot slot in wireSlots)
        {
            slot.GetWire().DisableRaycast();
        }
    }

    internal void EnableRaycasts()
    {
        foreach (WireSlot slot in wireSlots)
        {
            slot.GetWire().EnableRaycast();
        }
    }
}
