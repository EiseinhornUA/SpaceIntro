using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NumPad: MonoBehaviour
{ 
    [System.Serializable]
    public class ButtonPair
    { 
        public string symbol;
        public Button button;
    }

    [SerializeField] private List<ButtonPair> buttonPairs;
    [SerializeField] private TextMeshProUGUI symbolEntry;
    [SerializeField] private string password;
    [SerializeField] private UnityEvent onAccessGranted;

    private UniTaskCompletionSource accessGranted;

    [SerializeField] private GameObject numPadInteractable;

    private void Awake()
    {
        foreach (var pair in buttonPairs)
        {
            pair.button.onClick.AddListener(() => OnButtonClick(pair.symbol));
        }
    }

    private void OnEnable()
    {
        Hud.Instance.HideHud();
        if (accessGranted == null)
            accessGranted = new UniTaskCompletionSource();
    }

    private void OnDisable()
    {
        Hud.Instance.ShowHud();
    }

    private async void OnButtonClick(string symbol)
    {
        if (symbol == "#")
        {
            if (symbolEntry.text == password)
            {
                symbolEntry.text = "SUCCESS";
                await UniTask.Delay(TimeSpan.FromSeconds(0.75f));
                symbolEntry.text = "";
                numPadInteractable.GetComponent<Interactable>().Deactivate();
                accessGranted.TrySetResult();
                OnAccessGranted();
            }

            else if (symbolEntry.text != "SUCCESS")
            {
                symbolEntry.text = "ERROR";
                await UniTask.Delay(TimeSpan.FromSeconds(0.75f));
                symbolEntry.text = "";
            }

            return;
        }

        if (symbol == "×")
        {
            if (!string.IsNullOrEmpty(symbolEntry.text))
            {
                symbolEntry.text = symbolEntry.text.Substring(0, symbolEntry.text.Length - 1);
            }

            return;
        }

        if (symbolEntry.text.Length >= 4)
        {
            return;
        }

        symbolEntry.text += symbol;
    }

    public void OnAccessGranted()
    {
        onAccessGranted.Invoke();
        gameObject.SetActive(false);
    }

    public async UniTask WaitUntilFinished()
    {
        await accessGranted.Task;
    }

    public async UniTask StartNumpad()
    {
        await WaitUntilFinished();
    }
}