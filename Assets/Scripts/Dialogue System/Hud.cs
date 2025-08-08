using System;
using UnityEngine;

public class Hud : MonoBehaviour
{
    internal void HideHud()
    {
        gameObject.SetActive(false);
    }

    internal void ShowHud()
    {
        gameObject.SetActive(true);
    }
}