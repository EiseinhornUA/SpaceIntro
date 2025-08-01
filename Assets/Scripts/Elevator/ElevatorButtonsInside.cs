using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElevatorButtonsInside : MonoBehaviour
{
    [SerializeField]
    private Elevator elevator;

    [SerializeField]
    private ElevatorDoors currentElevatorDoor;

    [SerializeField]
    private List<ElevatorDoorPair> elevatorDoors;

    [Serializable]
    public class ElevatorDoorPair
    {
        public ElevatorDoors elevatorDoor;
        public int floor;
    }

 
    public async UniTask ElevateToFloor(int destinationFloor)
    {
        await currentElevatorDoor.CloseDoors();
        await elevator.GoToFloor(destinationFloor);
        ElevatorDoors nextElevatorDoor = elevatorDoors.Find(door => door.floor == destinationFloor).elevatorDoor;
        await nextElevatorDoor.OpenDoors();
        currentElevatorDoor = nextElevatorDoor;
    }

    [ContextMenu("Elevate to Floor 0")]
    public void ElevateToFloor0()
    {
        ElevateToFloor(0).Forget();
    }

    [ContextMenu("Elevate to Floor 1")]
    public void ElevateToFloor1()
    {
        ElevateToFloor(1).Forget();
    }

    [ContextMenu("Elevate to Floor 2")]
    public void ElevateToFloor2()
    {
        ElevateToFloor(2).Forget();
    }

    public void SetCurrentFloor(int currentFloor)
    {
        Debug.Log($"Setting current elevatorDoor to {currentFloor}");
        currentElevatorDoor = elevatorDoors.Find(door => door.floor == currentFloor).elevatorDoor;
    }
}
