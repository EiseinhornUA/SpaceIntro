using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CableMiniGame : MonoBehaviour
{
    private const int size = 6;
    [SerializeField] private List<WireSlot> wireSlots;

    private Wire GetWire(Vector2Int position)
    {
        return wireSlots[position.x + (position.y * size)].GetWire();
    }
}
