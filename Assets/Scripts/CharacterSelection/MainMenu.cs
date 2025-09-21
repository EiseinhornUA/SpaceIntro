using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

internal class MainMenu : Popup
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button profileButton;

    public UnityEvent OnGameStarted { get; private set; } = new();
    public UnityEvent OnProfileButtonClicked { get; private set; } = new();

    private void Start()
    {
        startButton.onClick.AddListener(() => OnGameStarted.Invoke());
        profileButton.onClick.AddListener(() => OnProfileButtonClicked.Invoke());
    }
}