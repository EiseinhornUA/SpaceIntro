using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorButtonsInside : MonoBehaviour
{
    [SerializeField]
    private ElevatorControlPanel elevatorControlPanel;

    public async UniTask ElevateToFloor(int floor)
    {
        await elevatorControlPanel.CallElevator();
    }
}
