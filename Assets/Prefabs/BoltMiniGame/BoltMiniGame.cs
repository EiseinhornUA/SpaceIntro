using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoltMiniGame : MonoBehaviour
{
    private UnityEngine.Vector2 clickPosition;

    private Wall wall;
    private Hole[] holes;
    public Hole holeFrom;
    public Hole holeTo;
    private Plank[] planks;

    [SerializeField] private float clickOnHoleThreshold = 0.25f;
    [SerializeField] private float plankHoleCheckThreshold = 0.2f;
    [SerializeField] public float speedToSwapBolts = 6f;
    [SerializeField] public Ease boltSwapEase = Ease.InOutQuint;

    private void Awake()
    {
        wall = GetComponentInChildren<Wall>();
        holes = wall.holes;
        planks = wall.planks;
        foreach (Plank plank in wall.planks)
            plank.plankHoleCheckThreshold = plankHoleCheckThreshold;

        foreach (Plank plank in planks)
        {
            plank.AttachAllBolts();
        }
    }

    private async void Update()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            UnityEngine.Vector3 clickPositionInCanvas = Input.mousePosition;
            clickPositionInCanvas.z = transform.position.z;

            clickPosition = Camera.main.ScreenToWorldPoint(clickPositionInCanvas);

            if (!holeFrom)
            {
                holeFrom = GetClickedHole();
                return;
            }
                
            if (!holeTo)
            {
                holeTo = GetClickedHole();
            }

            if (holeFrom && holeTo)
            {
                await SwapBolts(holeFrom, holeTo);
                this.holeFrom = null;
                this.holeTo = null;
            }
        }

        foreach (Hole hole in holes)
        {
            if (IsAbleToPlace(hole))
            {
                hole.GetComponent<SpriteRenderer>().color = Color.green;
            }
            else
            {
                hole.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }

    }

    private Hole GetClickedHole()
    {
        foreach (var hole in holes)
        {
            float clickToHoleDistance = UnityEngine.Vector2.Distance(clickPosition, hole.transform.position);

            if (clickToHoleDistance < clickOnHoleThreshold)
            {
                return hole;
            }
        }
        return null;
    }

    [ContextMenu("InitializeGame")]
    private void InitializeGame()
    {

    }

    private IEnumerable<Bolt> GetBolts()
    {
        return wall.GetBolts();
    }

    //private Hole GetNearestHole(Vector2 position)
    //{
    //    return holes
    //                .Where(h => Vector2.Distance(position, h.transform.position) < checkBoltRadius)
    //                .FirstOrDefault();
    //}

    [ContextMenu("SwapBolts")]
    private async UniTask SwapBoltsWithArguments()
    {
        await SwapBolts(holeFrom, holeTo);
        this.holeFrom = null;
        this.holeTo = null;
    }

    private async UniTask SwapBolts(Hole holeFrom, Hole holeTo)
    {
        if (!holeFrom.HasBolt()) return;

        if (holeTo.HasBolt()) return;

        if (!IsAbleToPlace(holeTo)) return;

        foreach (Plank plank in planks)
            plank.DetachBolt(holeFrom);

        holeTo.PlaceBolt(holeFrom.GetBolt());

        //foreach (Plank plank in planks)
        //    plank.HolesPlankIsMoveable(holeTo, false);

        await holeTo.MoveBolt(holeFrom.GetBolt());

        //foreach (Plank plank in planks)
        //    plank.HolesPlankIsMoveable(holeTo, true);

        foreach (Plank plank in planks)
            plank.AttachToBolt(holeTo);

        holeFrom.RemoveBolt();
    }

    private bool IsAbleToPlace(Hole holeTo)
    {
        return !planks.Any(p => p.IsBlocking(holeTo));
    }
}
