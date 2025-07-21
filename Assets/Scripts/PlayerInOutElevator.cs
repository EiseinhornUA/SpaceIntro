using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;

public class PlayerInOutElevator : MonoBehaviour
{
    [SerializeField] private Elevator elevator;
    [SerializeField] private List<ElevatorControlPanel> elevatorControlPanels;
    [SerializeField] private ElevatorButtonsInside elevatorButtonsInside;
    [SerializeField] private Player player;
    [SerializeField] private Transform playersPointOutsideElevator;
    [SerializeField] private Transform playersPointInsideElevator;

    public async UniTask MovePlayerToFloor(int floorFrom, int floorTo)
    {
        await GetCurrentElevatorPanel(floorFrom).CallElevator();

        // Animator playerAnimator = player.GetComponent<Animator>();
        // playerAnimator.Play("Walking");

        player.SetGravityEnabled(false);

        Vector3 playerPositionInsideElevator = playersPointInsideElevator.position;//transform.Find("ElevatorCenter");

        await player.transform.DOMove(playerPositionInsideElevator, 1.0f);

        player.SetGravityEnabled(true);

        // playerAnimator.Play("Breathing Idle");

        await elevatorButtonsInside.ElevateToFloor(floorTo);

        player.SetGravityEnabled(false);
        // playerAnimator.Play("Walking");

        //Transform elevatorOutside = elevator.transform.Find("ElevatorOutside");
        Vector3 destinationOutSideElevator = playersPointOutsideElevator.position;
        await player.transform.DOMove(destinationOutSideElevator, 1.0f).AsyncWaitForCompletion();

        player.SetGravityEnabled(true);

        // playerAnimator.Play("Breathing Idle");
    }

    private ElevatorControlPanel GetCurrentElevatorPanel(int currentFloor)
    {
        return elevatorControlPanels.Find(cp => cp.GetPanelFloor() == currentFloor);
    }

    [ContextMenu("Go to Floor 1 to 2")]
    public void GoToFloor2()
    {
        MovePlayerToFloor(1, 2).Forget();
    }
}
