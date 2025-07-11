using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;

[UnitTitle("Dialogue Node")]
[UnitCategory("Dialogue")]
public class DialogueNode : WaitUnit
{
    private ValueInput messageInput;
    private ValueInput characterInput;
    private DialogueView view;

    protected override void Definition()
    {
        base.Definition();
        messageInput = ValueInput<string>("Dialogue Line", "");
        characterInput = ValueInput<DialogueCharacter>("Character", null);
        Succession(enter, exit);
    }

    protected override IEnumerator Await(Flow flow)
    {
        view = GameObject.FindObjectOfType<DialogueView>();

        var message = flow.GetValue<string>(messageInput);
        var character = flow.GetValue<DialogueCharacter>(characterInput);

        view.ChangeCharacterName(character.GetName());
        view.ChangeCharacterPortrait(character.GetPortrait());
        view.ChangeMessage(message);

        yield return view.WaitForClick().ToCoroutine();
        yield return exit;
    }
}
