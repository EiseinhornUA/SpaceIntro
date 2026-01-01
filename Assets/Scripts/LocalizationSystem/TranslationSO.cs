using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Localization/Translation Data")]
public class TranslationSO : ScriptableObject
{
    public SystemLanguage language;

    [System.Serializable]
    public struct Entry
    {
        public string key;
        [TextArea] public string value;
    }

    public List<Entry> entries;

    Dictionary<string, string> dict;

    public void Init()
    {
        dict = new Dictionary<string, string>();
        foreach (var e in entries)
            dict[e.key] = e.value;
    }

    public string Get(string key)
    {
        if (dict == null) Init();
        return dict.TryGetValue(key, out var v) ? v : key;
    }
}
