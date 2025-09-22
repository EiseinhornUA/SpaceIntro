using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class BrokenSlidingDoor : SlidingDoor
{
    [SerializeField] public float openPercent = 20f;

    [ContextMenu("Open Broken Door")]

    public override void OpenDoor() => PlayBrokenDoorAnimationAsync().Forget();

    public async UniTask PlayBrokenDoorAnimationAsync()
    {
        if (locked)
        {
            return;
        }
        if (!isOpen)
        {
            gameObject.GetComponent<SoundPlayer>().Play();
            await doorMoveablePart.transform.DOLocalMoveY(openLength * (openPercent / 100f), duration * (openPercent / 100f));
            isOpen = true;
        }
    }
}
