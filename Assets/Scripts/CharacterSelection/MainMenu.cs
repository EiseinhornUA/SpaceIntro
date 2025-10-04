using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

internal class MainMenu : Popup
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button profileButton;

    [Header("Popups")]
    [SerializeField] private PopupView newGamePopup;
    [SerializeField] private PopupView settingsPopup;
    [SerializeField] private PopupView profileLockedPopup;

    public UnityEvent OnGameStarted { get; private set; } = new();

    public UnityEvent OnSettingsButtonClicked { get; private set; } = new();
    public UnityEvent OnProfileButtonClicked { get; private set; } = new();

    private void Start()
    {
        startButton.onClick.AddListener(OnGameStarted.Invoke);
        settingsButton.onClick.AddListener(OnSettingsButtonClicked.Invoke);
        profileButton.onClick.AddListener(OnProfileButtonClicked.Invoke);

        OnSettingsButtonClicked.AddListener(settingsPopup.Show);

        profileButton.interactable = GameStateProvider.IsGameCompleted();
    }
}