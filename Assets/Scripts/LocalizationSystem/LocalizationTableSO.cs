using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Localization/Localization Table")]
public class LocalizationTableSO : ScriptableObject
{
    [System.Serializable]
    public struct LanguageValue
    {
        public SystemLanguage language;
        [TextArea] public string value;
    }

    [System.Serializable]
    public struct Entry
    {
        public string key;
        public List<LanguageValue> translations;
    }

    public List<Entry> entries;

    Dictionary<(string, SystemLanguage), string> dict;

    public void Init()
    {
        dict = new Dictionary<(string, SystemLanguage), string>();
        foreach (var e in entries)
            foreach (var t in e.translations)
                dict[(e.key, t.language)] = t.value;
    }

    public string Get(string key, SystemLanguage language)
    {
        if (dict == null) Init();
        return dict.TryGetValue((key, language), out var v) ? v : key;
    }
}
