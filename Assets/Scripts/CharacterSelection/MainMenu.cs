using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

internal class MainMenu : Popup
{
    [Header("Buttons")]
    [SerializeField] private Button continueButton;
    //[SerializeField] private Button startButton;
    [SerializeField] private Button startWarningButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button profileButton;
    [SerializeField] private Button creditsButton;

    [Header("Popups")]
    [SerializeField] private PopupView newGamePopup;
    //[SerializeField] private PopupView settingsPopup;
    [SerializeField] private PopupView profileLockedPopup;

    public UnityEvent OnContinueButtonClicked { get; private set; } = new();
    public UnityEvent OnGameStarted { get; private set; } = new();
    public UnityEvent OnSettingsButtonClicked { get; private set; } = new();
    public UnityEvent OnProfileButtonClicked { get; private set; } = new();
    public UnityEvent OnCreditsButtonClicked { get; private set; } = new();

    private void Start()
    {
        continueButton.onClick.AddListener(OnContinueButtonClicked.Invoke);
        startWarningButton.onClick.AddListener(OnGameStarted.Invoke);
        settingsButton.onClick.AddListener(OnSettingsButtonClicked.Invoke);
        profileButton.onClick.AddListener(OnProfileButtonClicked.Invoke);
        creditsButton.onClick.AddListener(OnCreditsButtonClicked.Invoke);

        //OnSettingsButtonClicked.AddListener(settingsPopup.Show);

        OnGameStarted.AddListener(GameStateProvider.SetStarted);

        continueButton.gameObject.SetActive(IsContinueAvaliable());
        
        profileButton.interactable = GameStateProvider.IsGameCompleted();
    }

    private static bool IsContinueAvaliable()
    {
        return GameStateProvider.IsGameStarted() || GameStateProvider.IsGameContinued();
    }
}