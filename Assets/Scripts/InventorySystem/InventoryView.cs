using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class InventoryView : MonoBehaviour
{
    [Header("Inventory References")]
    private List<ItemSlot> itemSlots = new List<ItemSlot>();
    [SerializeField] private ItemSlot itemSlotPrefab;
    [SerializeField] private Transform itemsParent;
    [SerializeField] private GameObject ItemDescriptionPanel;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemDescriptionText;
    [Header("Task References")]
    [SerializeField] private TaskView taskView;

    private void Start()
    {
        foreach (var itemSlot in itemSlots)
        {
            itemSlot.OnItemSelected += OnItemSelect;
        }
    }

    private void OnItemSelect(ItemSlot selectedItem)
    {
        ItemDescriptionPanel.SetActive(true);
        SetItemName(selectedItem);
        SetItemDescription(selectedItem);
    }

    private void SetItemName(ItemSlot itemSlot)
    {
        itemNameText.text = itemSlot.GetItem().itemName;
    }

    private void SetItemDescription(ItemSlot itemSlot)
    {
        itemDescriptionText.text = itemSlot.GetItem().description;
    }

    private void ClearItems()
    {
        foreach (Transform child in itemsParent)
        {
            Destroy(child.gameObject);
        }
        itemSlots.Clear();
    }

    internal void AddItem(InventoryItem inventoryItem)
    {
        ItemSlot item = Instantiate(itemSlotPrefab, itemsParent);
        item.SetItem(inventoryItem);
        itemSlots.Add(item);
        item.OnItemSelected += OnItemSelect;
    }

    internal void SetTaskText(string text)
    {
        if (!taskView) return;
        taskView.SetTaskText(text);
    }
}
