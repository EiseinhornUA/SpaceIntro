using Unity.VisualScripting;
using UnityEngine;

[UnitTitle("End Dialogue Node")]
[UnitCategory("Dialogue")]
public class EndDialogueNode : Unit
{
    private ControlInput enter;
    private ControlOutput exit;

    protected override void Definition()
    {
        exit = ControlOutput("exit");
        enter = ControlInput("enter", OnEnter);
        Succession(enter, exit);
    }

    private ControlOutput OnEnter(Flow flow)
    {
        GameObject.FindObjectOfType<DialogueView>().gameObject.SetActive(false);
        return exit;
    }
}
