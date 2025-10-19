using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreditsScreen : Popup
{
    [SerializeField] private Button backButton;

    public UnityEvent OnBackButtonClicked { get; private set; } = new();

    private void Start()
    {
        backButton.onClick.AddListener(OnBackButtonClicked.Invoke);
    }
}