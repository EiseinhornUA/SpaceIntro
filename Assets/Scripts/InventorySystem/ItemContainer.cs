using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;


public class ItemContainer : MonoBehaviour
{

    [SerializeField] private List<InventoryItem> items = new();
    [SerializeField] private BackpackView backpackView;

    public void AddItem(ItemSO itemSO)
    {
        if (itemSO == null)
        {
            throw new System.ArgumentNullException(nameof(itemSO), "Item cannot be null.");
        }
        items.Add(itemSO.AsInventoryItem());
        backpackView.AddItem(itemSO.AsInventoryItem());
    }

    internal bool HasItem(string name)
    {
        return items.Contains(items.Find(item => item.itemName == name));
    }

    internal bool HasItem(ItemSO itemSO)
    {
        if (itemSO == null)
        {
            throw new System.ArgumentNullException(nameof(itemSO), "Item cannot be null.");
        }
        return items.Contains(itemSO.AsInventoryItem());
    }

    public void RemoveItem(ItemSO itemSO) => items.Remove(itemSO.AsInventoryItem());

    public InventoryItem GetItem(GameObject gameObject)
    {
        return items.Find(item => item.itemName == gameObject.name);
    }

    [ContextMenu("Print Items")]
    public void PrintItems()
    {
        Debug.Log(string.Join('\n', items.Select(item => $"{item.itemName}")));
    }
}