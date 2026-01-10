using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;

public class LocalizationManager
{
    private const string LanguageKey = "Language";
    private List<LocalizedTMP> texts = new();
    private static LocalizationManager _instance;
    public static LocalizationTableSO localizationTable { get; private set; }
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
        localizationTable = table;

        currentLanguage = language;
        SaveCurrentLanguage();

        foreach (var t in texts)
        {
            if (!t) continue;
            if (!table.entries.Select(e => e.key).Contains(t.key))
            {
                Debug.LogError($"key '{t.key}' not found in localization table");
                continue;
            }

            t.Translate();
        }

    }

    private void SaveCurrentLanguage()
    {
        PlayerPrefs.SetInt(LanguageKey, ((int)currentLanguage));
    }

    public void LoadCurrentLanguage(LocalizationTableSO table)
    {
        if (!table)
        {
            throw new ArgumentNullException(nameof(table));
        }

        ChangeLanguage(table, GetCurrentLanguage());
    }

    public static SystemLanguage GetCurrentLanguage()
    {
        int langValue = PlayerPrefs.GetInt(LanguageKey, (int)SystemLanguage.English);

        return (SystemLanguage)langValue;
    }

    public static string Translate(string text)
    {
        return localizationTable.Get(text.Replace("\r\n", "\n"), GetCurrentLanguage());
    }
}
