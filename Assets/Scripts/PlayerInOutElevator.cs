using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;

public class PlayerInOutElevator : MonoBehaviour
{
    [SerializeField] private Elevator elevator;
    [SerializeField] private List<ElevatorControlPanel> elevatorControlPanels;
    [SerializeField] private ElevatorButtonsInside elevatorButtonsInside;
    [SerializeField] private Player player;
    [SerializeField] private Animator player_animator;
    [SerializeField] private Transform playersPointOutsideElevator;
    [SerializeField] private Transform playersPointInsideElevator;
    
    private Animator animator;
    [SerializeField] private Transform characterParent;

    private Transform modelTransform;

    public async UniTask RotatePlayerTowardsElevator()
    {
        modelTransform = GetModelTransform();

        Vector3 direction = (playersPointInsideElevator.position - modelTransform.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        //targetRotation *= Quaternion.Euler(0, 0f, 0);

        await modelTransform
            .DORotateQuaternion(targetRotation, 0.25f)
            .AsyncWaitForCompletion();
    }

    private Transform GetModelTransform()
    {
        return characterParent.GetChild(1).GetChild(0).transform;
    }

    public async UniTask RotatePlayerTowardExitOfElevator()
    {
        Vector3 direction = (playersPointOutsideElevator.position - modelTransform.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation *= Quaternion.Euler(0, 0f, 0);

        await modelTransform
            .DORotateQuaternion(targetRotation, 0.25f)
            .AsyncWaitForCompletion();
    }


    public async UniTask MovePlayerToFloor(int floorFrom, int floorTo)
    {
        await GetCurrentElevatorPanel(floorFrom).CallElevator();

        await RotatePlayerTowardsElevator();
        EnablePlayerControls(false);
        GetAnimationHandler().SetHorizontalSpeed(1f);

        player.SetGravityEnabled(false);

        Vector3 playerPositionInsideElevator = playersPointInsideElevator.position;

        await player.transform.DOMove(playerPositionInsideElevator, 1.0f);

        await RotatePlayerTowardExitOfElevator();

        player.transform.SetParent(elevator.transform);

        player.SetGravityEnabled(true);

        GetAnimationHandler().SetHorizontalSpeed(0f);

        await elevatorButtonsInside.ElevateToFloor(floorTo);

        player.transform.SetParent(null);

        player.SetGravityEnabled(false);

        GetAnimationHandler().SetHorizontalSpeed(1f);

        Vector3 destinationOutSideElevator = playersPointOutsideElevator.position;
        await player.transform.DOMove(destinationOutSideElevator, 1.0f).AsyncWaitForCompletion();

        player.SetGravityEnabled(true);

        GetAnimationHandler().SetHorizontalSpeed(0f);
        EnablePlayerControls(true);
    }

    private static void EnablePlayerControls(bool enabled)
    {
        GameObject.FindAnyObjectByType<PlayerRotator>().enabled = enabled;
        GameObject.FindAnyObjectByType<Player>().enabled = enabled;
    }

    private static AnimationHandler GetAnimationHandler()
    {
        return GameObject.FindAnyObjectByType<AnimationHandler>();
    }

    private ElevatorControlPanel GetCurrentElevatorPanel(int currentFloor)
    {
        return elevatorControlPanels.Find(cp => cp.GetPanelFloor() == currentFloor);
    }

    [ContextMenu("GoToFloor1")]

    public void GoToFloor1()
    {
        MovePlayerToFloor(2, 0).Forget();
    }
    
    [ContextMenu("GoToFloor2")]

    public void GoToFloor2()
    {
        MovePlayerToFloor(0, 1).Forget();
    }

    [ContextMenu("GoToFloor3")]

    public void GoToFloor3()
    {
        MovePlayerToFloor(1, 2).Forget();
    }

    public void GoToFloor1From2()
    {
        MovePlayerToFloor(1, 0).Forget();
    }

    [ContextMenu("RotateTowardsElevator")]
    private void RotateTowardsElevator()
    {
        RotatePlayerTowardsElevator().Forget();
    }
}
