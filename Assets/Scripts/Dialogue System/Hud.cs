using System;
using UnityEngine;

public class Hud : MonoBehaviour
{
    internal static Hud FindHud()
    {
        return FindObjectOfType<Hud>(includeInactive: true);
    }

    internal void HideHud()
    {
        gameObject.SetActive(false);
    }

    internal void ShowHud()
    {
        gameObject.SetActive(true);
    }
}