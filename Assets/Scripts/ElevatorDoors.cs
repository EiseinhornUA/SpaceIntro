using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoors : MonoBehaviour
{

    [SerializeField]
    private float elevatorOpenDistance = 0.7f;

    [SerializeField]
    private float openDuration = 0.75f;

    [SerializeField]
    private GameObject leftDoor;

    [SerializeField]
    private GameObject rightDoor;

    [SerializeField]
    private Ease ease = Ease.Linear;

    private bool isElevatorOpened;

    [ContextMenu("Open door")]
    public async UniTask OpenDoors()
    {
        if (!isElevatorOpened)
        {
            _ = leftDoor.transform.DOMoveX(elevatorOpenDistance, openDuration).SetEase(ease).SetRelative();
            await rightDoor.transform.DOMoveX(-elevatorOpenDistance, openDuration).SetEase(ease).SetRelative();
        }
        isElevatorOpened = true;
    }

    [ContextMenu("Close door")]
    public async UniTask CloseDoors()
    {
        if (isElevatorOpened)
        {
            _ = leftDoor.transform.DOMoveX(-elevatorOpenDistance, openDuration).SetEase(ease).SetRelative();
            await rightDoor.transform.DOMoveX(elevatorOpenDistance, openDuration).SetEase(ease).SetRelative();
        }
        isElevatorOpened = false;
    }

    public bool IsElevatorOpened() => isElevatorOpened;
}
