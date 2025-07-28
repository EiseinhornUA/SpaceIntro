using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class ItemSlot : MonoBehaviour
{
    private InventoryItem item;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    public event Action<ItemSlot> OnItemSelected = delegate { };
    internal void SetItem(InventoryItem item)
    {
        this.item = item;
        icon.sprite = item.icon;
    }

    internal InventoryItem GetItem() => item;

    private void Awake()
    {
        button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        OnItemSelected(this);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClicked);
    }
}

