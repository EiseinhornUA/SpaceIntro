using UnityEngine;

public static class ItemExtensions
{
    public static InventoryItem AsInventoryItem(this ItemSO ItemSO)
    {
        return new InventoryItem(ItemSO);
    }
}
