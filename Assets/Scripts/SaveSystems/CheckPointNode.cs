using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CheckPointNode : WaitUnit
{
    public ValueInput checkpointNumberInput;

    protected override void Definition()
    {
        base.Definition();
        checkpointNumberInput = ValueInput<int>("Checkpoint Number", 0);
    }

    protected override IEnumerator Await(Flow flow)
    {
        var persistanceManager = GameObject.FindObjectOfType<PersistanceManager>();

        int checkpointNumber = flow.GetValue<int>(checkpointNumberInput);

        persistanceManager.Save(checkpointNumber);

        yield return exit;
    }
}