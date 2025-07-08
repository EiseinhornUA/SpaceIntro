using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

[UnitTitle("Start Dialogue Node")]
[UnitCategory("Quest")]
public class StartDialogueNode : Unit
{
    private ValueInput dialogueInput;
    private ControlInput enter;
    private ControlOutput exit;

    protected override void Definition()
    {
        exit = ControlOutput("exit");
        enter = ControlInput("enter", OnEnter);
        dialogueInput = ValueInput<DialoguePrefab>("Dialogue Prefab", null);

        Succession(enter, exit);
    }

    private ControlOutput OnEnter(Flow flow)
    {
        var dialoguePrefab = flow.GetValue<DialoguePrefab>(dialogueInput);

        GameObject.FindObjectOfType<DialogueManager>().StartDialogue(dialoguePrefab);

        return exit;
    }
}
