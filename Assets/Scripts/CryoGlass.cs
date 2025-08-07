using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;

public class CryoGlass : MonoBehaviour
{
    [SerializeField] private Transform pivotPoint;
    private float openTime = 1.5f;
    private float targetAngle = 90f;

    public enum DoorState : int
    {
        CLOSED = 0,
        OPENED,
        OPENING,
        CLOSING
    }

    [SerializeField]
    private DoorState doorState;

    void Update()
    {
        float currentYRotation = pivotPoint.localEulerAngles.y;

        if (doorState == DoorState.OPENING)
        {
            if(Mathf.DeltaAngle(currentYRotation, targetAngle) > 0.1f)
            {
                Opening();
            }
            else
            {
                doorState = DoorState.OPENED;
            }
        }

        if (doorState == DoorState.CLOSING)
        {
            if (Mathf.DeltaAngle(currentYRotation, 0) < -0.1f)
            {
                Closing();
            }
            else
            {
                doorState = DoorState.CLOSED;
            }
        }

    }

    public void Opening()
    {
        pivotPoint.transform.RotateAround(pivotPoint.position, pivotPoint.up, 
            Time.deltaTime * (1f / openTime * targetAngle));
    }

    public void Closing()
    {
        pivotPoint.transform.RotateAround(pivotPoint.position, pivotPoint.up,
            -Time.deltaTime * (1f / openTime * targetAngle));
    }

    public async UniTask OpenAsync()
    {
        if (doorState == DoorState.OPENED)
            return;
        Open();
        await UniTask.WaitUntil(() => doorState == DoorState.OPENED);
    }

    public async UniTask CloseAsync()
    {
        if (doorState == DoorState.CLOSED)
            return;
        Close();
        await UniTask.WaitUntil(() => doorState == DoorState.CLOSED);
    }

    [ContextMenu("Open Glass")]
    private void Open()
    {
        doorState = DoorState.OPENING;
    }

    [ContextMenu("Close Glass")]
    private void Close()
    {
        doorState = DoorState.CLOSING;
    }
}
