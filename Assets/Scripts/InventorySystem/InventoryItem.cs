using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public string itemName { get; private set; }
    public Sprite icon { get; private set; }
    public string description { get; private set; }

    public InventoryItem(ItemSO itemSO)
    {
        itemName = itemSO.GetName();
        icon = itemSO.icon;
        description = itemSO.description;
    }
}