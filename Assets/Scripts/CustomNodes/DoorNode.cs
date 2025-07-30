using Unity.VisualScripting;
using UnityEngine;

[UnitTitle("Door Locker Node")]
[UnitCategory("Door System")]
public class DoorNode : Unit
{
    private ControlInput enter;
    private ControlOutput exit;
    private ValueInput doorInput;
    private ValueInput lockedState;

    protected override void Definition()
    {
        enter = ControlInput("enter", flow =>
        {
            var door = flow.GetValue<SlidingDoor>(doorInput);

            door.locked = flow.GetValue<bool>(lockedState);
            return exit;
        });

        exit = ControlOutput("exit");
        doorInput = ValueInput<SlidingDoor>("Door", default);
        lockedState = ValueInput<bool>("Locked", false);

        Succession(enter, exit);
        Requirement(doorInput, enter);
        Requirement(lockedState, enter);
    }
}
