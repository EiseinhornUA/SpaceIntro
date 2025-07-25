using UnityEngine;

public static class ItemExtensions
{
    public static Item AsItem(this GameObject gameObject)
    {
        return new Item(gameObject);
    }
}
