using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BoltMiniGame : MonoBehaviour
{
    private Vector2 clickPosition;

    [SerializeField] private Wall wall;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            clickPosition = Input.mousePosition;
        }
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

    private void SwapBolts(Hole holeFrom, Hole holeTo)
    {
        if (!holeFrom.HasBolt()) return;

        if (holeTo.HasBolt()) return;

        if (!IsAbleToPlace(holeTo)) return;
        holeTo.PlaceBolt(holeFrom.GetBolt());
        holeTo.RemoveBolt();
    }

    private bool IsAbleToPlace(Hole holeTo)
    {
        return true;
    }
}
