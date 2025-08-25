using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BackpackView : MonoBehaviour
{
    [SerializeField] private ItemSlot itemSlotPrefab;
    [SerializeField] private Transform itemsParent;
    
    [SerializeField] private ItemDescriptionPanel descriptionPanel;

    
    private List<ItemSlot> itemSlots = new List<ItemSlot>();

    private void OnItemSelect(ItemSlot selectedItem)
    {
        descriptionPanel.Show();
        descriptionPanel.SetItemName(selectedItem);
        descriptionPanel.SetItemDescription(selectedItem);
    }

    public void AddItem(InventoryItem inventoryItem)
    {
        ItemSlot item = Instantiate(itemSlotPrefab, itemsParent);
        item.SetItem(inventoryItem);
        itemSlots.Add(item);
        item.OnItemSelected += OnItemSelect;
    }

    private void ClearItems()
    {
        foreach (Transform child in itemsParent)
        {
            Destroy(child.gameObject);
        }
        itemSlots.Clear();
    }
}
