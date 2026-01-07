using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using TMPro;

[CustomEditor(typeof(LocalizationTableSO))]
public class LocalizationTableSOEditor : Editor
{
    List<TextMeshProUGUI> foundTexts;
    Dictionary<TextMeshProUGUI, bool> toggles = new();

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RenderCSVButtons();
        RenderFindLocalizedTMPsButton();

        GUILayout.Space(10);
        GUILayout.Label("TMP Auto Localization", EditorStyles.boldLabel);

        if (GUILayout.Button("Scan Scene TextMeshProUGUI"))
            ScanScene();

        if (foundTexts == null || foundTexts.Count == 0)
            return;

        GUILayout.Space(5);
        GUILayout.Label("Scene Texts", EditorStyles.miniBoldLabel);

        foreach (var tmp in foundTexts)
        {
            if (tmp == null) continue;

            if (!toggles.ContainsKey(tmp))
                toggles[tmp] = tmp.GetComponent<LocalizedTMP>() != null;

            EditorGUILayout.BeginHorizontal();

            toggles[tmp] = EditorGUILayout.Toggle(toggles[tmp], GUILayout.Width(18));

            if (GUILayout.Button($"\"{tmp.text}\"", EditorStyles.label))
            {
                Selection.activeObject = tmp.gameObject;
                EditorGUIUtility.PingObject(tmp.gameObject);
            }

            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("Apply Selection"))
            ApplySelection();
    }

    private void RenderFindLocalizedTMPsButton()
    {
        GUILayout.Space(10);
        GUILayout.Label("Scene Utilities", EditorStyles.boldLabel);

        if (GUILayout.Button("Find LocalizedTMPs in Scene → Add Keys"))
            AddKeysFromScene((LocalizationTableSO)target);
    }

    private void RenderCSVButtons()
    {
        GUILayout.Space(10);
        GUILayout.Label("CSV Import / Export", EditorStyles.boldLabel);

        if (GUILayout.Button("Export Pipe CSV"))
            ExportCSV((LocalizationTableSO)target);

        if (GUILayout.Button("Import Pipe CSV"))
            ImportCSV((LocalizationTableSO)target);
    }

    private void ExportCSV(LocalizationTableSO table)
    {
        var path = EditorUtility.SaveFilePanel(
            "Export Localization CSV",
            "",
            table.name + ".csv",
            "csv"
        );
        if (string.IsNullOrEmpty(path)) return;

        var languages = table.entries
            .SelectMany(e => e.translations)
            .Select(t => t.language)
            .Distinct()
            .ToList();

        using var writer = new StreamWriter(path);

        // Header
        writer.Write("key");
        foreach (var lang in languages)
            writer.Write("|" + lang);
        writer.WriteLine();

        // Rows
        foreach (var entry in table.entries)
        {
            var safeKey = entry.key.Replace("\"", "\"\"");
            writer.Write("\"" + safeKey + "\"");

            foreach (var lang in languages)
            {
                var v = entry.translations
                    .FirstOrDefault(t => t.language == lang).value ?? "";

                // CSV-safe: quotes + escape quotes
                v = v.Replace("\"", "\"\"");
                writer.Write("|\"" + v + "\"");
            }

            writer.WriteLine();
        }

        AssetDatabase.Refresh();
    }

    private void ImportCSV(LocalizationTableSO table)
    {
        var path = EditorUtility.OpenFilePanel("Import Localization CSV", "", "csv");
        if (string.IsNullOrEmpty(path)) return;

        var csv = File.ReadAllText(path);
        var rows = ParsePipeCSV(csv);

        if (rows.Count < 2) return;

        var languages = rows[0]
            .Skip(1)
            .Select(h => (SystemLanguage)System.Enum.Parse(typeof(SystemLanguage), h))
            .ToList();

        var entries = new List<LocalizationTableSO.Entry>();

        for (int i = 1; i < rows.Count; i++)
        {
            var row = rows[i];
            if (row.Count == 0 || string.IsNullOrWhiteSpace(row[0])) continue;

            var entry = new LocalizationTableSO.Entry
            {
                key = row[0],
                translations = new List<LocalizationTableSO.LanguageValue>()
            };

            for (int j = 1; j < row.Count && j <= languages.Count; j++)
            {
                entry.translations.Add(new LocalizationTableSO.LanguageValue
                {
                    language = languages[j - 1],
                    value = row[j]
                });
            }

            entries.Add(entry);
        }

        Undo.RecordObject(table, "Import Localization CSV");
        table.entries = entries;
        EditorUtility.SetDirty(table);
    }

    private void AddKeysFromScene(LocalizationTableSO table)
    {
        var localizedTmps = FindObjectsOfType<LocalizedTMP>(true);

        if (localizedTmps.Length == 0)
        {
            Debug.Log("No LocalizedTMP components found in scene.");
            return;
        }

        var existingKeys = new HashSet<string>(
            table.entries.Select(e => e.key)
        );

        var newEntries = new List<LocalizationTableSO.Entry>();

        foreach (var l in localizedTmps)
        {
            if (string.IsNullOrWhiteSpace(l.key)) continue;
            if (existingKeys.Contains(l.key)) continue;

            newEntries.Add(new LocalizationTableSO.Entry
            {
                key = l.key,
                translations = new List<LocalizationTableSO.LanguageValue>()
            });

            existingKeys.Add(l.key);
        }

        if (newEntries.Count == 0)
        {
            Debug.Log("All LocalizedTMP keys already exist.");
            return;
        }

        Undo.RecordObject(table, "Add Localization Keys From Scene");
        table.entries.AddRange(newEntries);
        EditorUtility.SetDirty(table);

        Debug.Log($"Added {newEntries.Count} new localization keys.");
    }

    void ScanScene()
    {
        foundTexts = FindObjectsOfType<TextMeshProUGUI>(true).Where(t => t.text.Length > 1).ToList();
        toggles.Clear();

        foreach (var tmp in foundTexts)
            toggles[tmp] = tmp.GetComponent<LocalizedTMP>() != null;
    }

    void ApplySelection()
    {
        foreach (var pair in toggles)
        {
            var tmp = pair.Key;
            var shouldHave = pair.Value;

            if (tmp == null) continue;

            var loc = tmp.GetComponent<LocalizedTMP>();

            if (shouldHave)
            {
                if (loc != null) Undo.DestroyObjectImmediate(loc);

                Undo.AddComponent<LocalizedTMP>(tmp.gameObject);
                loc = tmp.GetComponent<LocalizedTMP>();

                loc.SetKey(tmp.text);

                EditorUtility.SetDirty(tmp);
                EditorUtility.SetDirty(loc);
            }
            else if (!shouldHave && loc != null)
            {
                Undo.DestroyObjectImmediate(loc);
            }
        }
    }

    private List<List<string>> ParsePipeCSV(string csv)
    {
        var rows = new List<List<string>>();
        var row = new List<string>();
        var cell = "";
        bool inQuotes = false;

        for (int i = 0; i < csv.Length; i++)
        {
            char c = csv[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < csv.Length && csv[i + 1] == '"')
                {
                    cell += '"'; // escaped quote
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
                continue;
            }

            if (c == '|' && !inQuotes)
            {
                row.Add(cell);
                cell = "";
                continue;
            }

            if ((c == '\n' || c == '\r') && !inQuotes)
            {
                if (cell.Length > 0 || row.Count > 0)
                {
                    row.Add(cell);
                    rows.Add(row);
                    row = new List<string>();
                    cell = "";
                }
                continue;
            }

            cell += c;
        }

        row.Add(cell);
        rows.Add(row);

        return rows;
    }

}

