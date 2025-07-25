using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ItemContainer : MonoBehaviour
{

    [SerializeField] private List<Item> items = new();

    public void AddItem(GameObject item)
    {
        if (item == null)
        {
            throw new System.ArgumentNullException(nameof(item), "Item cannot be null.");
        }
        items.Add(item.AsItem());
    }
    public void RemoveItem(GameObject item) => items.Remove(item.AsItem());

    public Item GetItem(GameObject gameObject)
    {
        return items.Find(item => item.itemName == gameObject.name);
    }

    [ContextMenu("Print Items")]
    public void PrintItems()
    {
        Debug.Log(string.Join('\n', items.Select(item => $"{item.itemName}")));
    }
}