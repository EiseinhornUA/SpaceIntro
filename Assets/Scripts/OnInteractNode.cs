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
        List<Interactable> interactables = GetInteractables(flow);
        foreach (var interactable in gameObjects)
        {
            Debug.Log("Waiting for interaction with" + flow.GetValue<GameObject>(interactable));
        }

        int index = 0;
        yield return UniTask.WhenAny(interactables.Select(i => i.WaitForInteraction())).ContinueWith(i => index = i).ToCoroutine();
        Debug.Log($"Interaction completed {index}");
        interactables.ForEach(i => i.Deactivate());
        yield return exits[index];
        Debug.Log($"Interaction exited {index}");
    }

    private List<Interactable> GetInteractables(Flow flow)
    {
        List<Interactable> interactables = new List<Interactable>();
        for (int i = 0; i < interactableCount; i++)
        {
            var go = flow.GetValue<GameObject>(gameObjects[i]);

            GameObject trigger;
            bool hasInteractable = false;
            if (go.transform.childCount > 0)
            {
                Transform transform = go.transform.Find(TriggerName);
                if (transform)
                {
                    trigger = transform.gameObject;
                    hasInteractable = trigger.TryGetComponent<Interactable>(out Interactable interactable);
                    if (hasInteractable)
                    {
                        interactable.Activate();
                        interactables.Add(interactable);
                    }
                }
            }
            if (!hasInteractable)
            {
                Interactable interactable;
                trigger = new GameObject(TriggerName);
                trigger.transform.SetParent(go.transform, false);
                interactable = trigger.AddComponent<Interactable>();
                interactable.Activate();
                interactables.Add(interactable);
            }
        }

        return interactables;
    }
}
