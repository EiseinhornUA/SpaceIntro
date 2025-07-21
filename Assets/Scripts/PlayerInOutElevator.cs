using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerInOutElevator : MonoBehaviour
{
    [SerializeField]
    Elevator elevator;

    [SerializeField]
    ElevatorControlPanel elevatorControlPanel;

    [SerializeField]
    Player player;

    public async UniTask PutPlayerInElevator()
    {
        await elevatorControlPanel.CallElevator();

        _ = player.transform.DOMove(elevator.transform.position, 0.1f);
    }

}
