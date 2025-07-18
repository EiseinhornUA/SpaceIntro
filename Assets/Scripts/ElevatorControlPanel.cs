using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElevatorControlPanel : MonoBehaviour
{
    [SerializeField]
    private Elevator elevator;

    [SerializeField]
    private ElevatorDoors currentElevatorDoor;

    [SerializeField]
    private List<ElevatorDoors> elevatorDoors;

    [SerializeField]
    private int currentFloor = 0;

    [SerializeField]
    private ElevatorButtonsInside elevatorButtonsInside;

    [ContextMenu("Call Elevator")]
    public async UniTask CallElevator()
    {
        await UniTask.WhenAll(elevatorDoors.Where(door => door != currentElevatorDoor).Select(async door => await door.CloseDoors()));
        await elevator.GoToFloor(currentFloor);
        await currentElevatorDoor.OpenDoors();
        Debug.Log($"1111111111111Setting current elevatorDoor to {currentFloor}");
        elevatorButtonsInside.SetCurrentFloor(currentFloor);
    }
}
