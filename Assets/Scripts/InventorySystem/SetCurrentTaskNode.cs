using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


[UnitCategory("Inventory System")]
[UnitTitle("Set Current Task")]
public class SetCurrentTaskNode : Unit
{
    public ControlInput enter;
    public ControlOutput exit;
    public ValueInput inputText;

    protected override void Definition()
    {
        enter = ControlInput("enter", (flow) =>
        {
            InventoryView inventoryView = GameObject.FindObjectOfType<InventoryView>(includeInactive: true);
            string taskText = flow.GetValue<string>(inputText);
            if (!inventoryView)
            {
                throw new System.Exception("InventoryView not found in the scene.");
            }
            inventoryView.SetTaskText(taskText);
            return exit;
        });
        exit = ControlOutput("exit");
        inputText = ValueInput<string>("inputText", string.Empty);

        Succession(enter, exit);
        Requirement(inputText, enter);
    }
}
