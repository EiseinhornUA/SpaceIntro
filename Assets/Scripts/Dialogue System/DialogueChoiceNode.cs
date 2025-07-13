using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

[UnitTitle("Dialogue Choice Node")]
[UnitCategory("Dialogue")]
public class DialogueChoiceNode : WaitUnit
{
    [Inspectable]
    [UnitHeaderInspectable("Choice Count")]
    [Range(2, 4)]
    public int choiceCount = 2;

    private ValueInput messageInput;
    private ValueInput characterInput;

    private List<ValueInput> choiceInputs = new();
    private List<ControlOutput> exits = new();

    private DialogueView view;
    private ControlInput enter;

    protected override void Definition()
    {
        choiceInputs.Clear();
        exits.Clear();

        messageInput = ValueInput<string>("Dialogue Line", "");
        characterInput = ValueInput<DialogueCharacter>("Character", null);
        enter = ControlInputCoroutine("enter", Await);

        for (int i = 0; i < choiceCount; i++)
        {
            var input = ValueInput<string>($"Text{i + 1}", "");
            var exit = ControlOutput($"Choice {i + 1} Exit");

            choiceInputs.Add(input);
            exits.Add(exit);

            Succession(enter, exit);
        }
    }

    protected override IEnumerator Await(Flow flow)
    {
        view = GameObject.FindObjectOfType<DialogueView>();

        var message = flow.GetValue<string>(messageInput);
        var character = flow.GetValue<DialogueCharacter>(characterInput);

        if (character)
        {
            view.ChangeCharacterName(character.GetName());
            view.ChangeCharacterPortrait(character.GetPortrait());
            view.ChangeMessage(message);
        }
        else
        {
            Debug.LogError(character + " is null. Please assign a character to the Dialogue Choice Node.");
        }

            var choices = new List<string>();
        for (int i = 0; i < choiceCount; i++)
        {
            choices.Add(flow.GetValue<string>(choiceInputs[i]));
        }

        view.ShowChoices(choices);
        int selectedIndex = -1;
        UniTask<int> task = view.WaitForChoice();
        yield return task.ContinueWith(i => selectedIndex = i).ToCoroutine();

        yield return exits[selectedIndex];
    }
}
