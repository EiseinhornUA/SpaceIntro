using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

[CreateAssetMenu(menuName = "Localization/Localization Table")]
public class LocalizationTableSO : ScriptableObject
{
    private IEqualityComparer<string> comparer = new StringComparer();
    public class StringComparer : IEqualityComparer<string>
    {
        public bool Equals(string left, string right)
        {
            if (left.Length != right.Length)
            {
/*                if (left.Length == 414 && right.Length == 420)
                {
                    //Debug.LogError($"StringComparer: '{left}' != '{right}' length {left.Length} != {right.Length}");
                    Debug.LogError($"left: {left}");
                    Debug.LogError($"right: {right}");

                    for (int i = 0; i < left.Length; i++)
                    {
                        char cl = left[i];
                        char cr = right[i];

                        if (!char.IsLetter(cr) && !char.IsDigit(cr)) continue;
                        if (cl != cr)
                        {
                            Debug.LogError($"StringComparer: '{left}' != '{right}' at char '{cl}' != '{cr}' at position {i}");
                            return false;
                        }
                    }
                    return true;
                }
*/
                return false;
            }

            for (int i = 0; i < left.Length; i++)
            {
                char cl = left[i];
                char cr = right[i];

                if (!char.IsLetter(cr) && !char.IsDigit(cr)) continue;
                if (cl != cr)
                {
                    Debug.LogError($"StringComparer: '{left}' != '{right}' at char '{cl}' != '{cr}' at position {i}");
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }

    [System.Serializable]
    public struct LanguageValue
    {
        public SystemLanguage language;
        [TextArea] public string value;
    }

    [System.Serializable]
    public struct Entry
    {
        [TextArea] public string key;
        public List<LanguageValue> translations;
    }

    public List<Entry> entries;

    Dictionary<SystemLanguage, Dictionary<string, string>> localizedDict;

    public void Init()
    {
        localizedDict = new();
        foreach (var e in entries)
        {
            foreach (var t in e.translations)
            {
                if(!localizedDict.ContainsKey(t.language))
                {
                    localizedDict[t.language] = new(comparer);
                }
                localizedDict[t.language][e.key] = t.value;
            }
        }
    }

    public string Get(string key, SystemLanguage language)
    {
        /*        if (localizedDict == null) Init();
                if (!localizedDict.TryGetValue(language, out var table))
                {
                    Debug.LogError($"Language: {language} not found");
                }
                if (!table.TryGetValue(key, out var value))
                {
                    Debug.LogError($"key '{key}' not found in localization table");
                    return key;
                }
                return value;*/
        var index = entries.FindIndex(e => comparer.Equals(e.key, key));
        if (index == -1) return key;
        var translationIndex = entries[index].translations.FindIndex(t => t.language == language);
        if (translationIndex == -1) return key;
        return entries[index].translations[translationIndex].value;
    }

    public bool Contains(string key)
    {
        var index = entries.FindIndex(e => comparer.Equals(e.key, key));
        if (index == -1) return false;
        return true;
    }
}
