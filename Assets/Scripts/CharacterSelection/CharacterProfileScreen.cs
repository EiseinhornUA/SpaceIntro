using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterProfileScreen : Popup
{
    [SerializeField] private Button backButton;
    public UnityEvent onBackButtonClicked { get; private set; } = new();

    private void Start()
    {
        backButton.onClick.AddListener(() =>
        {
            onBackButtonClicked.Invoke();
        });
    }
}