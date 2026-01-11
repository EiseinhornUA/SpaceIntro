using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedTMP : MonoBehaviour
{
    [field: SerializeField, TextArea] public string key { get; private set; }

    public void Translate()
    {
        var textTMP = GetComponent<TextMeshProUGUI>();

        LocalizationTableSO table = LocalizationManager.localizationTable;

        if (!table)
        {
            throw new ArgumentNullException(nameof(table));
        }

        if (!table.Contains(key))
        {
            Debug.LogError($"key '{key}' of '{gameObject.name}' not found in localization table");
        }

        textTMP.text = table.Get(key, LocalizationManager.GetCurrentLanguage());
    }

    public void SetKey(string newKey) => key = newKey;
    public void SetTextAndKey(string text)
    {
        var tmp = GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        SetKey(tmp.text);
        Translate();
    }

    public void SetTextWithoutKey(string text)
    {
        var tmp = GetComponent<TextMeshProUGUI>();
        tmp.text = text;
    }

    public string GetText() => GetComponent<TextMeshProUGUI>().text;

    public void SetColor(Color color) => GetComponent<TextMeshProUGUI>().color = color;
}
