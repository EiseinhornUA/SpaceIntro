using System;
using TMPro;
using UnityEngine;

public class ItemDescriptionPanel : Popup
{
    [SerializeField] private TextMeshProUGUI itemNameTMP;
    [SerializeField] private TextMeshProUGUI itemDescriptionTMP;

    public void SetItemName(ItemSlot itemSlot)
    {
        itemNameTMP.text = itemSlot.GetItem().itemName;
    }

    public void SetItemDescription(ItemSlot itemSlot)
    {
        itemDescriptionTMP.text = itemSlot.GetItem().description;
    }
}