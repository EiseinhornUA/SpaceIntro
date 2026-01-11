using UnityEngine;
using UnityEditor;
using TMPro;
using System.Text.RegularExpressions;

public class TrimTMPEmptyLines
{
    [MenuItem("Tools/TextMeshPro/Trim Trailing Empty Lines")]
    static void TrimAllTMP()
    {
        var texts = Object.FindObjectsOfType<TextMeshProUGUI>(true);
        int changed = 0;

        foreach (var tmp in texts)
        {
            string original = tmp.text;
            string trimmed = TrimEndEmptyLines(original);

            if (original != trimmed)
            {
                Undo.RecordObject(tmp, "Trim TMP Empty Lines");
                tmp.text = trimmed;
                EditorUtility.SetDirty(tmp);
                changed++;
            }
        }

        Debug.Log($"Trimmed trailing empty lines in {changed} TextMeshProUGUI objects.");
    }

    static string TrimEndEmptyLines(string text)
    {
        // Removes empty lines (including whitespace) only at the end
        return Regex.Replace(text, @"(\r?\n\s*)+$", "");
    }
}

