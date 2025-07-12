using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

[UnitTitle("Start Dialogue Node")]
[UnitCategory("Quest")]
public class StartDialogueNode : WaitUnit
{
    private ValueInput dialogueInput;

    protected override void Definition()
    {
        base.Definition();
        dialogueInput = ValueInput<DialoguePrefab>("Dialogue Prefab", null);

        Succession(enter, exit);
    }

    protected override IEnumerator Await(Flow flow)
    {
        var dialoguePrefab = flow.GetValue<DialoguePrefab>(dialogueInput);

        DialogueManager dialogueManager = GameObject.FindObjectOfType<DialogueManager>();
        dialogueManager.StartDialogue(dialoguePrefab);

        yield return dialogueManager.WaitForDialogueEnd().ToCoroutine();

        yield return exit;
    }
}
