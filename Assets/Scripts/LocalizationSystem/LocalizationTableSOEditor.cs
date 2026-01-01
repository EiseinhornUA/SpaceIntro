using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(LocalizationTableSO))]
public class LocalizationTableSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);
        GUILayout.Label("CSV Import / Export", EditorStyles.boldLabel);

        if (GUILayout.Button("Export Pipe CSV"))
            ExportCSV((LocalizationTableSO)target);

        if (GUILayout.Button("Import Pipe CSV"))
            ImportCSV((LocalizationTableSO)target);

        GUILayout.Space(10);
        GUILayout.Label("Scene Utilities", EditorStyles.boldLabel);

        if (GUILayout.Button("Find LocalizedTMPs in Scene → Add Keys"))
            AddKeysFromScene((LocalizationTableSO)target);
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
            writer.Write(entry.key);
            foreach (var lang in languages)
            {
                var v = entry.translations
                    .FirstOrDefault(t => t.language == lang).value ?? "";
                writer.Write("|" + v.Replace("\n", "\\n"));
            }
            writer.WriteLine();
        }

        AssetDatabase.Refresh();
    }

    private void ImportCSV(LocalizationTableSO table)
    {
        var path = EditorUtility.OpenFilePanel("Import Localization CSV", "", "csv");
        if (string.IsNullOrEmpty(path)) return;

        var lines = File.ReadAllLines(path);
        if (lines.Length < 2) return;

        var headers = lines[0].Split('|');
        var languages = headers
            .Skip(1)
            .Select(h => (SystemLanguage)System.Enum.Parse(typeof(SystemLanguage), h))
            .ToList();

        var entries = new List<LocalizationTableSO.Entry>();

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            var cells = lines[i].Split('|');
            var entry = new LocalizationTableSO.Entry
            {
                key = cells[0],
                translations = new List<LocalizationTableSO.LanguageValue>()
            };

            for (int j = 1; j < cells.Length && j <= languages.Count; j++)
            {
                entry.translations.Add(new LocalizationTableSO.LanguageValue
                {
                    language = languages[j - 1],
                    value = cells[j].Replace("\\n", "\n")
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
}

