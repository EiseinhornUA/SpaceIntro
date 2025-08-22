using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class InventoryView : Popup
{
    [SerializeField] private Button closeButton;

    private void Start()
    {
        closeButton.onClick.AddListener(Hide);
    }

    public override void Hide()
    {
        base.Hide();
        Hud.Instance.ShowHud();
    }
}
