using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine.Events;

[UnitTitle("Pickup On Interact Node")]
[UnitCategory("Inventory")]
public class PickUpOnInteractNode : WaitUnit
{
    private ValueInput gameObjectInput;
    private GameObject itemObject;
    private ValueInput itemInput;
    private ControlInput enter;
    private ControlOutput exitNoItem;
    private ControlOutput exitHasItem;

    private PopupManager popupManager;
    private ItemPickUpPopUp itemPickUpPopUp;

    private Interactable interactable;
    private ValueInput disableAfterInteraction;

    protected override void Definition()
    {
        enter = ControlInputCoroutine("", Await);
        exitNoItem = ControlOutput("NoItem");
        exitHasItem = ControlOutput("HasItem");
        gameObjectInput = ValueInput<GameObject>("gameObject", default);
        itemInput = ValueInput<ItemSO>("itemSO", default);

        Succession(enter, exitNoItem);
        Succession(enter, exitHasItem);

        disableAfterInteraction = ValueInput<bool>("Disable After Interaction", true);
        Requirement(disableAfterInteraction, enter);
    }

    protected override IEnumerator Await(Flow flow)
    {
        ItemContainer itemContainer = GameObject.FindObjectOfType<ItemContainer>(includeInactive: true);
        ItemSO item = flow.GetValue<ItemSO>(itemInput);
        if (itemContainer.HasItem(item.GetName()))
            yield return exitHasItem;

        interactable = flow.GetValue<Interactable>(gameObjectInput);
        interactable.Activate();
        yield return interactable.WaitForInteraction().ToCoroutine();

        itemObject = flow.GetValue<GameObject>(gameObjectInput);
        if (!HasPersistentMethod(interactable.onInteract, nameof(ItemPickUp.PickUpItem)))
        {
            popupManager = GameObject.FindObjectOfType<PopupManager>();
            itemPickUpPopUp = popupManager.ShowPopup<ItemPickUpPopUp>();

            itemContainer.AddItem(item);
            itemObject.SetActive(false);
            itemPickUpPopUp.ShowPickedUpItem(item, itemObject.transform);

            if (flow.GetValue<bool>(disableAfterInteraction))
                interactable.Deactivate();

            yield return itemPickUpPopUp.WaitForMove().ToCoroutine();
        }
        yield return exitNoItem;
    }

    private bool HasPersistentMethod(UnityEvent evt, string methodName)
    {
        int count = evt.GetPersistentEventCount();

        for (int i = 0; i < count; i++)
            if (evt.GetPersistentMethodName(i) == methodName)
                return true;

        return false;
    }

}
