using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

[UnitTitle("Pickup Item Node")]
[UnitCategory("Inventory")]
public class PickupItemNode : WaitUnit
{
    private ValueInput gameObjectInput;
    private ValueInput itemInput;
    private ControlInput enter;
    private ControlOutput exit;

    private PopupManager popupManager;
    private ItemPickUpPopUp itemPickUpPopUp;

    protected override void Definition()
    {
        enter = ControlInputCoroutine("", Await);
        exit = ControlOutput("");
        gameObjectInput = ValueInput<GameObject>("gameObject", default);
        itemInput = ValueInput<ItemSO>("itemSO", default);
        Succession(enter, exit);
    }

    protected override IEnumerator Await(Flow flow)
    {
        popupManager = GameObject.FindObjectOfType<PopupManager>();
        itemPickUpPopUp = popupManager.ShowPopup<ItemPickUpPopUp>();

        ItemContainer itemContainer = GameObject.FindObjectOfType<ItemContainer>(includeInactive: true);
        ItemSO item = flow.GetValue<ItemSO>(itemInput);
        itemContainer.AddItem(item);
        flow.GetValue<GameObject>(gameObjectInput).SetActive(false);
        itemPickUpPopUp.ShowPickedUpItem(flow, itemInput, gameObjectInput);
        yield return itemPickUpPopUp.WaitForMove().ToCoroutine();
        yield return exit;
    }
}
