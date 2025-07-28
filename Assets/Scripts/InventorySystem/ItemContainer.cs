using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;


public class ItemContainer : MonoBehaviour
{

    [SerializeField] private List<InventoryItem> items = new();
    [SerializeField] private InventoryView inventoryView;

    public void AddItem(ItemSO itemSO)
    {
        if (itemSO == null)
        {
            throw new System.ArgumentNullException(nameof(itemSO), "Item cannot be null.");
        }
        items.Add(itemSO.AsInventoryItem());
        inventoryView.AddItem(itemSO.AsInventoryItem());
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