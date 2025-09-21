using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
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

    [SerializeField] private Vector3 impulseForce;
    [SerializeField] private ParticleSystem escapeParticles;

    [ContextMenu("Escape")]
    public void Escape() => EscapeAsync().Forget();

    public async UniTask EscapeAsync()
    {
        modelTransform = player.GetModelTransform().parent;

        player.EnableControls(false);

        Hud hud = Hud.Instance;

        hud.HideHud();

        escapeParticles.Play();
        gameObject.GetComponent<SoundPlayer>().Play();
        await cryoGlass.OpenAsync();

        RotatePlayerToExit();

        gameObject.AddComponent<Rigidbody>();

        AddImpulse();

        player.StartWalkingAnimation();

        await player.transform.DOMove(PosOutsideCryoChamber.position, escapeTime);

        player.StopWalkingAnimation();

        hud.ShowHud();

        player.EnableControls(true);
    }

    [ContextMenu("Add Impulse")]
    private void AddImpulse()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(impulseForce, ForceMode.Impulse);
    }

    public void RotatePlayerToExit() => RotatePlayerToExitAsync().Forget();

    private async UniTask RotatePlayerToExitAsync()
    {
        await modelTransform.DORotate(new Vector3(0f, 0f, 0f), rotateTime);
    }
}
