using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


[UnitCategory("Inventory System")]
[UnitTitle("Set Current Task")]
public class SetCurrentTaskNode : Unit
{
    public ControlInput enter;
    public ControlOutput exit;
    public ValueInput inputName;
    public ValueInput inputDescription;

    protected override void Definition()
    {
        enter = ControlInput("enter", (flow) =>
        {
            JournalView journalView = GameObject.FindObjectOfType<JournalView>(includeInactive: true);
            
            if (!journalView)
            {
                throw new System.Exception("BackpackView not found in the scene.");
            }

            string taskName = flow.GetValue<string>(inputName);
            string taskDescription = flow.GetValue<string>(inputDescription);

            journalView.AddTask(taskName, taskDescription);
            
            return exit;
        });

        exit = ControlOutput("exit");
        inputName = ValueInput<string>("inputName", string.Empty);
        inputDescription = ValueInput<string>("inputDescription", string.Empty);

        Succession(enter, exit);
        Requirement(inputName, enter);
        Requirement(inputDescription, enter);
    }
}
