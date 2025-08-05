using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPlayerToOutsideAnimation : MonoBehaviour
{
    [SerializeField] PlayerInOutElevator playerMethod;
    [SerializeField] Player player;
    [SerializeField] private Transform characterParent;
    [SerializeField] float shiftDuration = 4f;
    [SerializeField] float shiftDistance = -10f;
    [SerializeField] ItemContainer itemContainer;
    [SerializeField] ItemSO inventoryItem;

    private Transform modelTransform;

    //public void ShiftPlayerInEnd()
    //{
    //    if (itemContainer.HasItem(inventoryItem.name))
    //    {
    //        Play();
    //    }
    //}

    public void Play() => PlayAsync().Forget();

    public async UniTask PlayAsync()
    {
        player.EnableControls(false);
        modelTransform = player.GetModelTransform();

        await modelTransform.DORotate(new Vector3(0f, 180f, 0f), 0.75f);

        player.GetAnimationHandler().SetHorizontalSpeed(1f);

        await player.transform.DOMove(new Vector3(
            player.transform.position.x, 
            player.transform.position.y, 
            player.transform.position.z + shiftDistance), shiftDuration);

        player.GetAnimationHandler().SetHorizontalSpeed(0f);
        //playerMethod.EnablePlayerControls(true);
    }
}
