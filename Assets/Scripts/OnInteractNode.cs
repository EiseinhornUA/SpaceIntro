using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

[UnitTitle("On Interact Node")]
[UnitCategory("Quest")]
public class OnInteractNode : WaitUnit
{
    [UnitHeaderInspectable("Interactable Count")]
    [Range(1, 10)]
    public int interactableCount = 1;

    private ControlInput enter;
    private List<ValueInput> gameObjects = new();
    private List<ControlOutput> exits = new();
    private ValueInput disableAfterInteraction;

    protected override void Definition()
    {
        enter = ControlInputCoroutine("enter", Await);


        for (int i = 0; i < interactableCount; i++)
        {
            var input = ValueInput<GameObject>($"GameObject {i + 1}", null);
            gameObjects.Add(input);

            var output = ControlOutput($"exit {i + 1}");
            exits.Add(output);

            Succession(enter, output);
        }

        disableAfterInteraction = ValueInput<bool>("Disable After Interaction", true);

        Requirement(disableAfterInteraction, enter);
    }

    protected override IEnumerator Await(Flow flow)
    {
        for (int i = 0; i < interactableCount; i++)
        {
            var go = flow.GetValue<GameObject>(gameObjects[i]);

            Interactable interactable = go.AddComponent<Interactable>();

            yield return interactable.WaitForInteraction().ToCoroutine();

            if (flow.GetValue<bool>(disableAfterInteraction))
                interactable.Deactivate();

            yield return exits[i];
        }
    }
}
