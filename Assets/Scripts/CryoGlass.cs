using DG.Tweening;
using UnityEngine;

public class CryoGlass : MonoBehaviour
{
    [SerializeField] private Transform pivotPoint;
    private float openSpeed = 1.5f;
    private float targetAngle = 90f;

    public enum DoorState : int
    {
        IDLE = 0,
        OPENING = 1,
        CLOSING = 2
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
                doorState = DoorState.IDLE;
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
                doorState = DoorState.IDLE;
            }
        }

    }

    public void Opening()
    {
        pivotPoint.transform.RotateAround(pivotPoint.position, pivotPoint.up, 
            Time.deltaTime * (1f / openSpeed * targetAngle));
    }

    public void Closing()
    {
        pivotPoint.transform.RotateAround(pivotPoint.position, pivotPoint.up,
            -Time.deltaTime * (1f / openSpeed * targetAngle));
    }

    [ContextMenu("Open Glass")]
    public void Open()
    {
        doorState = DoorState.OPENING;
    }

    [ContextMenu("Close Glass")]
    public void Close()
    {
        doorState = DoorState.CLOSING;
    }
}
