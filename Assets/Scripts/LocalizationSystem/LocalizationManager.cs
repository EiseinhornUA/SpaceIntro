using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public  class LocalizationManager
{
    private List<LocalizedTMP> texts = new();
    private static LocalizationManager _instance;
    private static SystemLanguage currentLanguage;

    public static LocalizationManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new LocalizationManager();
            return _instance;
        }
    }

    public void Register(LocalizedTMP t)
    {
        texts.Add(t);
    }

    public void ChangeLanguage(LocalizationTableSO table, SystemLanguage language)
    {
        foreach (var t in texts)
            t.Apply(table, language);
        currentLanguage = language;
    }

    public SystemLanguage GetCurrentLanguage() => currentLanguage;

    public void SetCurrentLanguage(SystemLanguage language) => currentLanguage = language;
}
