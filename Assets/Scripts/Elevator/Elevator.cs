using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> floors;

    [SerializeField]
    private float speed = 2f;

    public async UniTask GoToFloorAsync(int floor)
    {
        await gameObject.transform.DOMove(floors[floor].transform.position, speed).SetSpeedBased();
    }

    [ContextMenu("Go to Floor 0")]
    public void GoToFloor0()
    {
        GoToFloorAsync(0).Forget();
    }

    [ContextMenu("Go to Floor 1")]
    public void GoToFloor1()
    {
        GoToFloorAsync(1).Forget();
    }

    [ContextMenu("Go to Floor 2")]
    public void GoToFloor2()
    {
        GoToFloorAsync(2).Forget();
    }

    public PlayerInOutElevator GetPlayerInOutElevator()
    {
        return gameObject.GetComponent<PlayerInOutElevator>();
    }
}
