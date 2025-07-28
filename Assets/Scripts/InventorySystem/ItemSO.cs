using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/Inventory/Item", order = 1)]
public class ItemSO : ScriptableObject
{
    public string GetName() => name;
    [field: SerializeField] public Sprite icon { get; private set; }

    [field: SerializeField, TextArea(3, 10)] public string description { get; private set; }
}