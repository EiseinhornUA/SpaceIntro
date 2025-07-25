using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item
{
    [field: SerializeField] public string itemName { get; set; }

    public Item(GameObject gameObject)
    {
        itemName = gameObject.name;
    }
}