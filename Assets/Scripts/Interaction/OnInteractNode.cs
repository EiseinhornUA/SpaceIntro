using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

[UnitTitle("On Interact Node")]
[UnitCategory("Quest")]
public class OnInteractNode : WaitUnit
{
    private const string TriggerName = "Trigger";
    [UnitHeaderInspectable("Interactable Count")]
    [Range(1, 10)]
    public int interactableCount = 1;

    private new ControlInput enter;
    private List<ValueInput> interactables = new();
    private List<ControlOutput> exits = new();
    private ValueInput disableAfterInteraction;

    protected override void Definition()
    {
        enter = ControlInputCoroutine("enter", Await);


        for (int i = 0; i < interactableCount; i++)
        {
            var input = ValueInput<Interactable>($"Interactable {i + 1}", default);
            interactables.Add(input);

            var output = ControlOutput($"exit {i + 1}");
            exits.Add(output);

            Succession(enter, output);
        }

        disableAfterInteraction = ValueInput<bool>("Disable After Interaction", true);

        Requirement(disableAfterInteraction, enter);
    }

    protected override IEnumerator Await(Flow flow)
    {
        List<Interactable> interactables = GetInteractables(flow);
        foreach (var interactable in this.interactables)
        {
            Debug.Log("Waiting for interaction with" + flow.GetValue<Interactable>(interactable));
        }

        int index = 0;
        yield return UniTask.WhenAny(interactables.Select(i => i.WaitForInteraction())).ContinueWith(i => index = i).ToCoroutine();
        Debug.Log($"Interaction completed {index}");

        if (flow.GetValue<bool>(disableAfterInteraction))
            interactables.ForEach(i => i.Deactivate());
        
        yield return exits[index];
        Debug.Log($"Interaction exited {index}");
    }

    private List<Interactable> GetInteractables(Flow flow)
    {
        List<Interactable> interactableList = new List<Interactable>();
        foreach (var interactableInput in interactables)
        {
            var interactable = flow.GetValue<Interactable>(interactableInput);
            interactable.Activate();
            interactableList.Add(interactable);
        }
        return interactableList;
    }
}
