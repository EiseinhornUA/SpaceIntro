using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System;
using Tayx.Graphy.Utils;


public class ItemContainer : MonoBehaviour
{

    [SerializeField] private List<InventoryItem> items = new();
    [SerializeField] private BackpackView backpackView;
    [Header("ItemsSOs")]
    [SerializeField] private List<ItemSO> itemSOs = new();
    private List<ItemGameObject> itemGameObjects = new();

    public void AddItem(ItemSO itemSO)
    {
        if (itemSO == null)
        {
            throw new System.ArgumentNullException(nameof(itemSO), "Item cannot be null.");
        }
        items.Add(itemSO.AsInventoryItem());
        backpackView.AddItem(itemSO.AsInventoryItem());
        itemGameObjects.Find(igo => igo.GetSO() == itemSO).gameObject.SetActive(false);
    }

    private void Awake()
    {
        itemGameObjects = FindObjectsOfType<ItemGameObject>().ToList();
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

    public List<InventoryItem> GetItems()
    {
        return items;
    }

    [ContextMenu("Print Items")]
    public void PrintItems()
    {
        Debug.Log(string.Join('\n', items.Select(item => $"{item.itemName}")));
    }

    public List<string> GetStoredItemNames()
    {
        return items.Select(i => i.itemName).ToList();
    }

    public void AddItemsByNames(List<string> itemNames)
    {
        foreach (string itemName in itemNames)
        {
            AddItem(itemSOs.Find(i => i.GetName() == itemName));
        }
    }
}