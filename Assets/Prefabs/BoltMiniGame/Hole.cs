using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField] private Bolt bolt;
    private float speedToSwapBolts;
    private BoltMiniGame boltMiniGame;
    private Ease boltSwapEase;

    private void Awake()
    {
        boltMiniGame = FindObjectOfType<BoltMiniGame>();
        speedToSwapBolts = boltMiniGame.speedToSwapBolts;
        boltSwapEase = boltMiniGame.boltSwapEase;
    }

    public bool HasBolt()
    {
        return bolt != null;
    }

    public async UniTask PlaceBolt(Bolt bolt)
    {
        this.bolt = bolt;
        await bolt.transform.DOMove(transform.position, speedToSwapBolts).SetSpeedBased().SetEase(boltSwapEase);
    }

    public void RemoveBolt()
    {
        this.bolt = null;
    }

    internal Bolt GetBolt()
    {
        return bolt;
    }

    [ContextMenu("Debug Choose HoleFrom")]
    private void DebugChoose1Hole()
    {
        boltMiniGame.holeFrom = this;
    }

    [ContextMenu("Debug Choose HoleTo")]
    private void DebugChoose2Hole()
    {
        boltMiniGame.holeTo = this;
    }
}
