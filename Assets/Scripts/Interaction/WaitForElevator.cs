using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class WaitForElevator : WaitUnit
{
    private ValueInput elevatorInput;


    protected override void Definition()
    {
        base.Definition();
        elevatorInput = ValueInput<PlayerInOutElevator>("ElevatorInput", null);
    }

    protected override IEnumerator Await(Flow flow)
    {
        var elevator = flow.GetValue<PlayerInOutElevator>(elevatorInput);
        yield return elevator.WaitForElevator();
    }
}