using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[UnitTitle("Pickup Item Node")]
[UnitCategory("Inventory")]
public class PickupItemNode : Unit
{
    private ValueInput gameObjectInput;
    private ValueInput itemInput;

    private ControlInput enter;
    private ControlOutput exit;

    protected override void Definition()
    {
        enter = ControlInput("", OnEnter);
        exit = ControlOutput("");

        gameObjectInput = ValueInput<GameObject>("gameObject", default);
        itemInput = ValueInput<ItemSO>("itemSO", default);

        Succession(enter, exit);
    }

    protected ControlOutput OnEnter(Flow flow)
    {
        ItemContainer itemContainer = GameObject.FindObjectOfType<ItemContainer>(includeInactive: true);

        if (itemContainer)
        {
            ItemSO item = flow.GetValue<ItemSO>(itemInput);
            itemContainer.AddItem(item);
            flow.GetValue<GameObject>(gameObjectInput).SetActive(false);
            Debug.Log($"Item {item.name} picked up.");

            ShowPickedUpItem(flow);
        }

        return exit;
    }

    private void ShowPickedUpItem(Flow flow)
    {
        PopupManager popupManager = GameObject.FindObjectOfType<PopupManager>();

        ItemPickUpPopUp itemPickUpPopUp = popupManager.ShowPopup<ItemPickUpPopUp>();
        Sprite icon = flow.GetValue<ItemSO>(itemInput).icon;
        itemPickUpPopUp.SetIcon(icon);
        //itemPickUpPopUp.SaveItemTransform(flow.GetValue<GameObject>(gameObjectInput).transform);
        itemPickUpPopUp.MoveIconToPosition(flow.GetValue<GameObject>(gameObjectInput).transform);
    }
}
