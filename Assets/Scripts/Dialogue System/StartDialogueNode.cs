using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using System.Linq;

[UnitTitle("Start Dialogue Node")]
[UnitCategory("Quest")]
public class StartDialogueNode : WaitUnit
{
    private ValueInput dialogueInput;
    //private List<ValueInput> nameInputs = new();
    private ControlInput enter;
    private List<ControlOutput> exits = new();

    [UnitHeaderInspectable("Exit Count")]
    [Range(1, 16)]
    public int exitCount = 1;

    protected override void Definition()
    {
        dialogueInput = ValueInput<DialoguePrefab>("Dialogue Prefab", default);

        enter = ControlInputCoroutine("enter", Await);

        for (int i = 0; i < exitCount; i++)
        {
            //nameInputs.Add(ValueInput<string>($"Name {i + 1}", $"Option Name {i + 1}"));
            var exit = ControlOutput($"Exit {i + 1}");
            exits.Add(exit);
            Succession(enter, exit);
        }
    }

    protected override IEnumerator Await(Flow flow)
    {
        var dialoguePrefab = flow.GetValue<DialoguePrefab>(dialogueInput);

        DialogueManager dialogueManager = GameObject.FindObjectOfType<DialogueManager>();
        Hud hud = Hud.Instance;
        if (!dialogueManager) Debug.LogError("DialogueManager not found in the scene. Please add a DialogueManager component to a GameObject.");
        dialogueManager.StartDialogue(dialoguePrefab);

        hud.HideHud();
        yield return dialogueManager.WaitForDialogueEnd().ToCoroutine();
        hud.ShowHud();

        int index = dialogueManager.GetSelectedDecision().GetIndex();
        index = Mathf.Clamp(index, 0, exitCount - 1);
        yield return exits[index];
    }
}
