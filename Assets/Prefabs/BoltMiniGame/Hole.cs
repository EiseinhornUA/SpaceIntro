using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField] private Bolt bolt;

    public bool HasBolt()
    {
        return bolt != null;
    }

    public void PlaceBolt(Bolt bolt)
    {
        this.bolt = bolt;
    }

    public void RemoveBolt()
    {
        bolt = null;
    }

    internal Bolt GetBolt()
    {
        return bolt;
    }
}
