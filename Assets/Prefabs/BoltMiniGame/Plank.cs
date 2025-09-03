using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        Bounds plankBounds = GetComponent<BoxCollider2D>().bounds;
        Bounds holeToBounds = holeTo.GetComponent<CircleCollider2D>().bounds;
        print($"plank bounds: {plankBounds}");
        print($"holeToBounds: {holeToBounds}");
            
        if (HasAtLeastOneCoaxialHole(holeTo))
            return false;

        return plankBounds.Intersects(holeToBounds);
    }

    private bool HasAtLeastOneCoaxialHole(Hole holeTo)
    {
        return holes.Where(p => Vector2.Distance(p.transform.position, holeTo.transform.position)
                    < plankHoleCheckThreshold).Any();
    }
}
