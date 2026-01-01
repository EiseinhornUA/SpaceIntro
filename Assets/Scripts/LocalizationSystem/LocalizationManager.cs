using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public  class LocalizationManager
{
    private List<LocalizedTMP> texts = new();
    private static LocalizationManager _instance;

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

    public void ChangeLanguage(TranslationSO so)
    {
        //currentTranslation = so;
        foreach (var t in texts)
            t.Apply(so);
    }
}
