using System;
using UnityEngine;

public class LocalizedTMPRegistrar : MonoBehaviour
{
    [SerializeField] LocalizationTableSO localizationTable;

    private void Awake()
    {
        RegisterAll();
        Translate();
    }

    private static void RegisterAll()
    {
        foreach (var localizedTMP in FindObjectsOfType<LocalizedTMP>(includeInactive: true))
        {
            if (localizedTMP.gameObject.GetComponent<RealtimeLocalizedTMPTranslator>() != null)
                continue;

            LocalizationManager.Instance.Register(localizedTMP);
        }
    }

    [ContextMenu("Translate")]
    private void Translate()
    {
        LocalizationManager.Instance.LoadCurrentLanguage(localizationTable);
    }

}
