using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCryoChamberEscape : MonoBehaviour
{
    private const float rotateTime = 0.25f;
    [SerializeField] private Player player;
    [SerializeField] private float escapeTime = 2f;
    private Transform modelTransform;
    [SerializeField] private Transform PosOutsideCryoChamber;
    [SerializeField] private CryoGlass cryoGlass;

    [ContextMenu("Escape")]
    public void Escape() => EscapeAsync().Forget();

    public async UniTask EscapeAsync()
    {
        modelTransform = player.GetModelTransform().parent;

        player.EnableControls(false);

        Hud hud = Hud.FindHud();

        hud.HideHud();

        await cryoGlass.OpenAsync();

        RotatePlayerToExit();

        player.StartWalkingAnimation();

        await player.transform.DOMove(PosOutsideCryoChamber.position, escapeTime);

        player.StopWalkingAnimation();

        hud.ShowHud();

        player.EnableControls(true);
    }

    public void RotatePlayerToExit() => RotatePlayerToExitAsync().Forget();

    private async UniTask RotatePlayerToExitAsync()
    {
        await modelTransform.DORotate(new Vector3(0f, 0f, 0f), rotateTime);
    }
}
