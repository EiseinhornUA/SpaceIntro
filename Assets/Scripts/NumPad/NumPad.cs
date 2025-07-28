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

    private void Awake()
    {
        foreach (var pair in buttonPairs)
        {
            pair.button.onClick.AddListener(() => OnButtonClick(pair.symbol));
        }
    }

    private void OnButtonClick(string symbol)
    {
        if (symbol == "#")
        {
            if (symbolEntry.text == password)
            {
                symbolEntry.text = "";
                onAccessGranted.Invoke();
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
}