using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Plank : MonoBehaviour
{
    private PlankHole[] holes;
    public float plankHoleCheckThreshold;

    private void Awake()
    {
        holes = GetComponentsInChildren<PlankHole>();
    }

    public bool IsBlocking(Hole holeTo)
    {
        BoxCollider2D plankCollider = GetComponent<BoxCollider2D>();
        CircleCollider2D holeToCollider = holeTo.GetComponent<CircleCollider2D>();
        //print($"plank bounds: {plankBounds}");
        //print($"holeToBounds: {holeToBounds}");
            
        if (HasAtLeastOneCoaxialHole(holeTo))
            return false;

        var holeToPlankDistance = plankCollider.Distance(holeToCollider);

        return holeToPlankDistance.isOverlapped;
    }

    private bool HasAtLeastOneCoaxialHole(Hole holeTo)
    {
        return holes.Where(p => Vector2.Distance(p.transform.position, holeTo.transform.position)
                    < plankHoleCheckThreshold).Any();
    }
}
