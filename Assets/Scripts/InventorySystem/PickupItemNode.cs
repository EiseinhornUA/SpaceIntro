using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

[UnitTitle("Pickup Item Node")]
[UnitCategory("Inventory")]
public class PickupItemNode : Unit
{
    private ValueInput gameObjectInput;

    private ControlInput enter;
    private ControlOutput exit;

    protected override void Definition()
    {
        enter = ControlInput("", OnEnter);
        exit = ControlOutput("");

        gameObjectInput = ValueInput<GameObject>("gameObject", default);

        Succession(enter, exit);
    }

    protected ControlOutput OnEnter(Flow flow)
    {
        ItemContainer itemContainer = GameObject.FindObjectOfType<ItemContainer>(includeInactive: true);

        GameObject gameObject = flow.GetValue<GameObject>(gameObjectInput);

        if (itemContainer != null)
        {
            itemContainer.AddItem(gameObject);
            gameObject.SetActive(false);
            Debug.Log($"Item {gameObject.name} picked up.");
        }

        return exit;
    }
}
