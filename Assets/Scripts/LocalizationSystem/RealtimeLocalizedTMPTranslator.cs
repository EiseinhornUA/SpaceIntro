using System;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(LocalizedTMP))]
public class RealtimeLocalizedTMPTranslator : MonoBehaviour
{
    public void Translate()
    {
        var localizedTMP = GetComponent<LocalizedTMP>();
        var textTMP = GetComponent<TextMeshProUGUI>();

        localizedTMP.SetKey(textTMP.text);
        localizedTMP.Apply(LocalizationManager.localizationTable, LocalizationManager.GetCurrentLanguage());
    }
}
