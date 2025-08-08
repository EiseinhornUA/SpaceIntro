using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PlayerInOutElevator : MonoBehaviour
{
    [SerializeField] private float playerWalkingDuration = 1.0f;
    [SerializeField] private Elevator elevator;
    [SerializeField] private List<ElevatorControlPanel> elevatorControlPanels;
    [SerializeField] private ElevatorButtonsInside elevatorButtonsInside;
    [SerializeField] private Player player;
    [SerializeField] private RobotFollow robot;
    [SerializeField] private Animator player_animator;
    [SerializeField] private Transform playersPointOutsideElevator;
    [SerializeField] private Transform playersPointInsideElevator;
    [SerializeField] private Transform robotsPointOutsideElevator;
    [SerializeField] private Transform robotsPointInsideElevator;
    [SerializeField] private float robotFlyingSpeed = 20f;
    [SerializeField] private float robotRotateSpeed = 0.3f;
    [SerializeField] private Transform characterParent;

    private Transform modelTransform;

    public async UniTask RotatePlayerTowardsElevator()
    {
        modelTransform = player.GetModelTransform();

        Vector3 direction = (playersPointInsideElevator.position - modelTransform.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        //targetRotation *= Quaternion.Euler(0, 0f, 0);

        await modelTransform
            .DORotateQuaternion(targetRotation, 0.25f)
            .AsyncWaitForCompletion();
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


    public async UniTask MoveEntitiesToFloor(int floorFrom, int floorTo)
    {
        //player.SetGravityEnabled(true);
        player.EnableControls(false);
        await GetCurrentElevatorPanel(floorFrom).CallElevator();

        await MoveRobotToElevator();

        await RotatePlayerTowardsElevator();

        StartPlayerWalkingAnimation();

        //player.SetGravityEnabled(false);

        Vector3 playerPositionInsideElevator = playersPointInsideElevator.position;

        await player.transform.DOMove(playerPositionInsideElevator, playerWalkingDuration);

        await RotatePlayerTowardExitOfElevator();

        player.transform.SetParent(elevator.transform);
        MakeRobotFollowElevator();

        //player.SetGravityEnabled(true);
        
        StopPlayerWalkingAnimation();

        await elevatorButtonsInside.ElevateToFloor(floorTo);

        player.transform.SetParent(null);
        StopRobotFolowingElevator();

        //player.SetGravityEnabled(false);

        StartPlayerWalkingAnimation();

        Vector3 destinationOutSideElevator = playersPointOutsideElevator.position;
        await player.transform.DOMove(destinationOutSideElevator, playerWalkingDuration).AsyncWaitForCompletion();

        //player.SetGravityEnabled(true);

        StopPlayerWalkingAnimation();
        await MoveRobotOutOfElevator();
        player.EnableControls(true);
    }

    private void StopRobotFolowingElevator()
    {
        if (robot.IsRobotOn())
        {
            robot.transform.SetParent(null);
        }
    }

    private void MakeRobotFollowElevator()
    {
        if (robot.IsRobotOn())
        {
            robot.transform.SetParent(elevator.transform);
        }
    }

    private void StopPlayerWalkingAnimation()
    {
        player.GetAnimationHandler().SetHorizontalSpeed(0f);
    }

    private void StartPlayerWalkingAnimation()
    {
        player.GetAnimationHandler().SetHorizontalSpeed(1f);
    }

    public async UniTask MoveRobotToElevator()
    {
        if (!robot.IsRobotOn())
            return;

        DisablePlayerFollowing();
        await RotateRobotToOutsidePoint();
        //await MoveRobotToOutsidePoint();
        Vector3[] path = new[] { robotsPointOutsideElevator.position, robotsPointInsideElevator.position };
        RotateRobotToInsidePoint().Forget();
        await robot.transform.DOPath(path, robotFlyingSpeed).SetSpeedBased().SetEase(Ease.InOutSine);
        //await MoveRobotToInsidePoint();
        await RotateRobotToOutsidePoint();
    }

    public async UniTask MoveRobotOutOfElevator()
    {
        if (robot.IsRobotOn())
        {
            await MoveRobotToOutsidePoint();
            EnablePlayerFollowing();
        }
    }

    public void DisablePlayerFollowing()
    {
        robot.TurnOffRobotFollowing();
    }

    public async UniTask RotateRobotToOutsidePoint()
    {
        await robot.transform.DOLookAt(robotsPointOutsideElevator.position, robotRotateSpeed);
    }

    public async UniTask MoveRobotToOutsidePoint()
    {
        await robot.transform.DOMove(robotsPointOutsideElevator.position, robotFlyingSpeed).SetSpeedBased();
    }

    public async UniTask RotateRobotToInsidePoint()
    {
        await robot.transform.DOLookAt(robotsPointInsideElevator.position, robotRotateSpeed);
    }

    public async UniTask MoveRobotToInsidePoint()
    {
        await robot.transform.DOMove(robotsPointInsideElevator.position, robotFlyingSpeed).SetSpeedBased();
    }

    public void EnablePlayerFollowing()
    {
        robot.TurnOnRobotFollowing();
    }

    private ElevatorControlPanel GetCurrentElevatorPanel(int currentFloor)
    {
        return elevatorControlPanels.Find(cp => cp.GetPanelFloor() == currentFloor);
    }

    [ContextMenu("GoToFloor1")]

    public void GoToFloor1()
    {
        MoveEntitiesToFloor(2, 0).Forget();
    }
    
    [ContextMenu("GoToFloor2")]

    public void GoToFloor2()
    {
        MoveEntitiesToFloor(0, 1).Forget();
    }

    [ContextMenu("GoToFloor3")]

    public void GoToFloor3()
    {
        MoveEntitiesToFloor(1, 2).Forget();
    }

    public void GoToFloor1From2()
    {
        MoveEntitiesToFloor(1, 0).Forget();
    }

    [ContextMenu("RotateTowardsElevator")]
    private void RotateTowardsElevator()
    {
        RotatePlayerTowardsElevator().Forget();
    }
}
