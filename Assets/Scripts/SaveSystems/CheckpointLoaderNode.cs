using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[UnitCategory("Custom")]
[UnitTitle("Checkpoint Loader Node")]
public class CheckpointLoaderNode : WaitUnit
{
    [DoNotSerialize]
    [PortLabelHidden]
    public ControlInput enter;

    [DoNotSerialize]
    public List<ControlOutput> exits = new();

    [Serialize]
    [Inspectable]
    [UnitHeaderInspectable("Exit Count")]
    public int exitCount = 2;

    protected override void Definition()
    {
        exits.Clear();

        enter = ControlInputCoroutine("enter", Await);

        for (int i = 0; i < exitCount; i++)
        {
            var exit = ControlOutput($"exit_{i}");
            exits.Add(exit);
        }

        foreach (var e in exits)
            Succession(enter, e);
    }

    protected override IEnumerator Await(Flow flow)
    {
        var persistanceManager = GameObject.FindObjectOfType<PersistanceManager>();

        persistanceManager.Load();

        yield return exits[persistanceManager.GetCurrentCheckpoint()];
    }
}
