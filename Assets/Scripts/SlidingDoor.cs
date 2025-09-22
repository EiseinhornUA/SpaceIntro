using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    [SerializeField] public float openLength = 3.0f;
    [field:SerializeField] public bool locked { get; set; } = true;
    [SerializeField] protected bool isOpen = false;
    [SerializeField] protected float duration = 0.5f;
    [SerializeField] protected GameObject doorMoveablePart;
    private Vector3 localDoorPosition;

    private void Awake()
    {
        localDoorPosition = doorMoveablePart.transform.localPosition;
    }

    [ContextMenu("Open Door")]

    public virtual void OpenDoor() => OpenDoorAsync().Forget();

    public async UniTask OpenDoorAsync()
    {
        if (locked)
        {
            return;
        }
        if (!isOpen)
        {
            gameObject.GetComponent<SoundPlayer>().Play();
            await doorMoveablePart.transform.DOLocalMoveY(openLength, duration);
            isOpen = true;
        }
    }

    [ContextMenu("Close Door")]

    public virtual void CloseDoor() => CloseDoorAsync().Forget();

    public virtual async UniTask CloseDoorAsync()
    {
        if (locked)
        {
            return;
        }
        if (isOpen)
        {
            gameObject.GetComponent<SoundPlayer>().Play();
            await doorMoveablePart.transform.DOLocalMoveY(localDoorPosition.y, duration);
            isOpen = false;
        }
    }

    public void UnlockDoor() => locked = false;
}
