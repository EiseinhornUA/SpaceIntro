using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCloseDoor : MonoBehaviour
{
    [SerializeField] private float openLength = 3.0f;
    [field:SerializeField] public bool unlocked { get; set; } = true;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private GameObject doorMoveablePart;
    private Vector3 localDoorPosition;

    private void Awake()
    {
        localDoorPosition = doorMoveablePart.transform.localPosition;
    }

    [ContextMenu("Open Door")]
    public async UniTask OpenDoor()
    {
        if (!unlocked)
        {
            return;
        }
        if (!isOpen)
        {
            await doorMoveablePart.transform.DOLocalMoveY(openLength, duration);
            isOpen = true;
        }
    }

    [ContextMenu("Close Door")]
    public async UniTask CloseDoor()
    {
        if (!unlocked)
        {
            return;
        }
        if (isOpen)
        {
            await doorMoveablePart.transform.DOLocalMoveY(localDoorPosition.y, duration);
            isOpen = false;
        }
    }
}
