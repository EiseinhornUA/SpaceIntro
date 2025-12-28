using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    private PopupManager popupManager;
    private ItemPickUpPopUp itemPickUpPopUp;
    private ItemContainer itemContainer;

    [SerializeField] private ItemSO item;
    [SerializeField] private GameObject itemObject;

    private void Awake()
    {
        popupManager = GameObject.FindObjectOfType<PopupManager>();
        itemContainer = GameObject.FindObjectOfType<ItemContainer>(includeInactive: true);
    }

    public void PickUpItem()
    {
        itemPickUpPopUp = popupManager.ShowPopup<ItemPickUpPopUp>();

        itemContainer.AddItem(item);
        itemObject.SetActive(false);
        itemPickUpPopUp.ShowPickedUpItem(item, itemObject.transform);
        //yield return itemPickUpPopUp.WaitForMove().ToCoroutine();
    }
}
