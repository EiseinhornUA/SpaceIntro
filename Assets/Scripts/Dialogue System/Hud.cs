using System;
using UnityEngine;

public class Hud : MonoBehaviour
{
    public static Hud Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindHud();
            }
            return instance;
        }
    }

    private static Hud instance;

    private static Hud FindHud()
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