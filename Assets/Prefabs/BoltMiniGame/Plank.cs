using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plank : MonoBehaviour
{
    private PlankHole[] holes;

    private void Awake()
    {
        holes = GetComponentsInChildren<PlankHole>();
    }
}
