using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

[UnitTitle("On Interact Node")]
[UnitCategory("Quest")]
public class OnInteractNode : WaitUnit
{
    private ValueInput gameObject;
    private ValueInput disableAfterInteraction;

    protected override void Definition()
    {
        base.Definition();

        gameObject = ValueInput<GameObject>("Game Object", null);
        disableAfterInteraction = ValueInput<bool>("Disable After Interaction", true);

        Succession(enter, exit);
    }

    protected override IEnumerator Await(Flow flow)
    {
        var interactable = flow.GetValue<GameObject>(gameObject);

        yield return interactable.AddComponent<Interactable>().WaitForInteraction().ToCoroutine();
        if (flow.GetValue<bool>(disableAfterInteraction))
            interactable.GetComponent<Interactable>().Deactivate();

        yield return exit;
    }
}
