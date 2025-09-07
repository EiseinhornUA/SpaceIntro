using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Hole[] holes;
    [field:SerializeField]
    public Plank[] planks { get; private set; }

    internal IEnumerable<Bolt> GetBolts()
    {
        return holes.Where(h => h.HasBolt())
            .Select(h => h.GetBolt());
    }

    private void Awake()
    {
        holes = GetComponentsInChildren<Hole>();
    }
}