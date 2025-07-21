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

    private bool isPlayerMovingInFrontOfElevator = false;

    private Transform modelTransform;

    public async UniTask RotatePlayerToInside()
    {
        modelTransform = characterParent.GetChild(1).GetChild(0).transform;

        Vector3 direction = (playersPointInsideElevator.position - modelTransform.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation *= Quaternion.Euler(0, 0f, 0);

        await modelTransform
            .DORotateQuaternion(targetRotation, 0.25f)
            .AsyncWaitForCompletion();
    }

    public async UniTask RotatePlayerToOutside()
    {
        Vector3 direction = (playersPointOutsideElevator.position - modelTransform.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation *= Quaternion.Euler(0, 0f, 0);

        await modelTransform
            .DORotateQuaternion(targetRotation, 0.25f)
            .AsyncWaitForCompletion();
    }

    private void Update()
    {
        animator = characterParent.GetChild(1).GetChild(0).GetComponent<Animator>();

        if (isPlayerMovingInFrontOfElevator == true)
        {
            GameObject.FindAnyObjectByType<AnimationHandler>().enabled = false;
            animator.SetFloat("HorizontalSpeed", 1f);
        }
        else
        {
            GameObject.FindAnyObjectByType<AnimationHandler>().enabled = true;
        }
    }


    public async UniTask MovePlayerToFloor(int floorFrom, int floorTo)
    {
        await GetCurrentElevatorPanel(floorFrom).CallElevator();

        //characterParent.rotation = 
        isPlayerMovingInFrontOfElevator = true;
        await RotatePlayerToInside();
        animator.SetFloat("HorizontalSpeed", 1f);

        player.SetGravityEnabled(false);

        Vector3 playerPositionInsideElevator = playersPointInsideElevator.position;

        await player.transform.DOMove(playerPositionInsideElevator, 1.0f);

        player.transform.SetParent(elevator.transform);

        player.SetGravityEnabled(true);

        animator.SetFloat("HorizontalSpeed", 0f);
        isPlayerMovingInFrontOfElevator = false;

        await elevatorButtonsInside.ElevateToFloor(floorTo);

        player.transform.SetParent(null);

        player.SetGravityEnabled(false);

        isPlayerMovingInFrontOfElevator = true;
        await RotatePlayerToOutside();
        animator.SetFloat("HorizontalSpeed", 1f);

        Vector3 destinationOutSideElevator = playersPointOutsideElevator.position;
        await player.transform.DOMove(destinationOutSideElevator, 1.0f).AsyncWaitForCompletion();

        player.SetGravityEnabled(true);

        isPlayerMovingInFrontOfElevator = false;
        animator.SetFloat("HorizontalSpeed", 0f);
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
}
