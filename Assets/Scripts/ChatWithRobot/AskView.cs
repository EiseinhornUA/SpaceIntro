using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AskView : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [field: SerializeField] public Button askButton { get; private set;}
    [SerializeField] private TextMeshProUGUI responeText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Hud hud;

    private void Start()
    {
        closeButton.onClick.AddListener(Hide);
        askButton.onClick.AddListener(HideAskButton);
    }

    private void HideAskButton()
    {
        askButton.gameObject.SetActive(false);
    }

    public void ShowAskButton()
    {
        askButton.gameObject.SetActive(true);
    }

    [ContextMenu("Show")]
    public void Show()
    {
        gameObject.SetActive(true);
        hud.HideHud();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        hud.ShowHud();
    }

    public void SetResponse(string response) => responeText.text = response;

    public void ClearInput() => inputField.text = string.Empty;

    public string GetQuestionText() => inputField.text.Trim();
}
