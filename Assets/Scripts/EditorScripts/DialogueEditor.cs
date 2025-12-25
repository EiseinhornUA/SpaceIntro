using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using Newtonsoft.Json.Linq;

public class DialogueEditorWindow : EditorWindow
{
    private Dictionary<string, DialogueData> dialogues = new Dictionary<string, DialogueData>();
    private Vector2 scrollPosition;
    private string dialoguesPath = "Assets/Dialogues"; // ← Change this if your path is different
    
    [System.Serializable]
    private class DialogueData
    {
        public string name;
        public string jsonData;
        public ScriptGraphAsset asset;
        public bool foldout = true;
    }

    [MenuItem("Tools/Dialogue Editor")]
    public static void ShowWindow()
    {
        GetWindow<DialogueEditorWindow>("Dialogue Editor");
    }

    private void OnEnable()
    {
        LoadDialogues();
    }

    private void LoadDialogues()
    {
        dialogues.Clear();
        
        string[] guids = AssetDatabase.FindAssets("t:ScriptGraphAsset", new[] { dialoguesPath });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ScriptGraphAsset asset = AssetDatabase.LoadAssetAtPath<ScriptGraphAsset>(path);
            
            if (asset != null)
            {
                string json = ExtractJson(asset);
                dialogues[asset.name] = new DialogueData
                {
                    name = asset.name,
                    jsonData = json,
                    asset = asset
                };
            }
        }
    }

    private string ExtractJson(ScriptGraphAsset asset)
    {
        SerializedObject so = new SerializedObject(asset);
        SerializedProperty dataProp = so.FindProperty("_data");
        SerializedProperty jsonProp = dataProp.FindPropertyRelative("_json");
        return jsonProp.stringValue;
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reload Dialogues"))
        {
            LoadDialogues();
        }
        if (GUILayout.Button("Save All"))
        {
            SaveAllDialogues();
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Export Combined JSON"))
        {
            ExportCombinedJson();
        }
        if (GUILayout.Button("Import Combined JSON"))
        {
            ImportCombinedJson();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Export to Pipe CSV (|)"))
        {
            ExportToPipeCsv();
        }
        if (GUILayout.Button("Import from Pipe CSV (|)"))
        {
            ImportFromPipeCsv();
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Loaded {dialogues.Count} dialogues from {dialoguesPath}", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        foreach (var kvp in dialogues.ToList())
        {
            DialogueData data = kvp.Value;
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            data.foldout = EditorGUILayout.Foldout(data.foldout, data.name, true);
            
            if (data.foldout)
            {
                EditorGUI.indentLevel++;
                
                EditorGUILayout.LabelField("JSON Data:", EditorStyles.boldLabel);
                EditorGUI.BeginChangeCheck();
                data.jsonData = EditorGUILayout.TextArea(data.jsonData, GUILayout.Height(200));
                
                if (EditorGUI.EndChangeCheck())
                {
                    dialogues[kvp.Key] = data;
                }
                
                if (GUILayout.Button($"Save {data.name}"))
                {
                    SaveDialogue(data);
                }
                
                EditorGUI.indentLevel--;
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        EditorGUILayout.EndScrollView();
    }

    private void SaveDialogue(DialogueData data)
    {
        if (data.asset == null) return;
        
        SerializedObject so = new SerializedObject(data.asset);
        SerializedProperty dataProp = so.FindProperty("_data");
        SerializedProperty jsonProp = dataProp.FindPropertyRelative("_json");
        jsonProp.stringValue = data.jsonData;
        so.ApplyModifiedProperties();
        
        EditorUtility.SetDirty(data.asset);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"Saved {data.name}");
    }

    private void SaveAllDialogues()
    {
        foreach (var data in dialogues.Values)
        {
            SaveDialogue(data);
        }
        Debug.Log("All dialogues saved");
    }

    // ── Combined JSON Export / Import ───────────────────────────────────────

    private void ExportCombinedJson()
    {
        JObject combined = new JObject();
        
        foreach (var kvp in dialogues)
        {
            try
            {
                JObject parsed = JObject.Parse(kvp.Value.jsonData);
                combined[kvp.Key] = parsed;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to parse {kvp.Key}: {e.Message}");
            }
        }
        
        string path = EditorUtility.SaveFilePanel("Export Combined JSON", "", "CombinedDialogues.json", "json");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, combined.ToString(Newtonsoft.Json.Formatting.Indented));
            Debug.Log($"Exported to {path}");
        }
    }

    private void ImportCombinedJson()
    {
        string path = EditorUtility.OpenFilePanel("Import Combined JSON", "", "json");
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            string jsonContent = File.ReadAllText(path);
            JObject combined = JObject.Parse(jsonContent);
            
            int importedCount = 0;
            int notFoundCount = 0;
            
            foreach (var property in combined.Properties())
            {
                string dialogueName = property.Name;
                
                if (dialogues.ContainsKey(dialogueName))
                {
                    dialogues[dialogueName].jsonData = property.Value.ToString(Newtonsoft.Json.Formatting.None);
                    importedCount++;
                }
                else
                {
                    Debug.LogWarning($"Dialogue '{dialogueName}' not found in loaded assets. Skipping.");
                    notFoundCount++;
                }
            }
            
            Debug.Log($"Import complete: {importedCount} dialogues imported, {notFoundCount} not found");
            
            if (importedCount > 0 && EditorUtility.DisplayDialog(
                "Import Complete", 
                $"Imported {importedCount} dialogues.\nSave changes now?", 
                "Save All", 
                "Later"))
            {
                SaveAllDialogues();
            }
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Import Failed", $"Error importing JSON: {e.Message}", "OK");
            Debug.LogError($"Import failed: {e}");
        }
    }

    // ── Pipe (|) delimited CSV Export / Import ───────────────────────────────

    private void ExportToPipeCsv()
    {
        string path = EditorUtility.SaveFilePanel("Export Dialogues to Pipe CSV", "", "dialogues_pipe.csv", "csv");
        if (string.IsNullOrEmpty(path)) return;

        using (var writer = new StreamWriter(path))
        {
            writer.WriteLine("DialogueName|NodeId|Character|DialogueLine");

            foreach (var dialogue in dialogues)
            {
                string dialogueName = dialogue.Key;
                try
                {
                    JObject json = JObject.Parse(dialogue.Value.jsonData);
                    JArray elements = (JArray)json["graph"]?["elements"];

                    if (elements == null) continue;

                    foreach (JObject element in elements)
                    {
                        string type = element["$type"]?.Value<string>();
                        if (type != "DialogueNode" && type != "DialogueChoiceNode") continue;

                        string nodeId = element["$id"]?.Value<string>() ?? "";
                        string character = element["defaultValues"]?["Character"]?["$content"]?.Value<string>() ?? "";
                        string line = element["defaultValues"]?["Dialogue Line"]?["$content"]?.Value<string>() ?? "";

                        // Escape pipe and newlines
                        line = line.Replace("|", "\\|").Replace("\r\n", "\\n").Replace("\n", "\\n");

                        writer.WriteLine($"{dialogueName}|{nodeId}|{character}|{line}");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"Failed to process {dialogueName} for CSV export: {ex.Message}");
                }
            }
        }

        Debug.Log($"Pipe-delimited CSV exported to: {path}");
    }

    private void ImportFromPipeCsv()
    {
        string path = EditorUtility.OpenFilePanel("Import Dialogues from Pipe CSV", "", "csv");
        if (string.IsNullOrEmpty(path)) return;

        int updatedCount = 0;

        var linesByDialogue = new Dictionary<string, List<(string nodeId, string character, string line)>>();

        string[] allLines = File.ReadAllLines(path);
        for (int i = 1; i < allLines.Length; i++) // skip header
        {
            string line = allLines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = line.Split('|');
            if (parts.Length < 4) continue;

            string dialogueName = parts[0];
            string nodeId = parts[1];
            string character = parts[2];

            // Join the rest (in case the line itself contained unescaped pipes - rare)
            string dialogueText = string.Join("|", parts, 3, parts.Length - 3);
            // Unescape
            dialogueText = dialogueText.Replace("\\|", "|").Replace("\\n", "\n");

            if (!linesByDialogue.TryGetValue(dialogueName, out var list))
            {
                list = new List<(string, string, string)>();
                linesByDialogue[dialogueName] = list;
            }

            list.Add((nodeId, character, dialogueText));
        }

        // Apply changes to JSON
        foreach (var kvp in linesByDialogue)
        {
            string dialogueName = kvp.Key;
            if (!dialogues.TryGetValue(dialogueName, out var data)) continue;

            try
            {
                JObject json = JObject.Parse(data.jsonData);
                JArray elements = (JArray)json["graph"]?["elements"];

                if (elements == null) continue;

                bool modified = false;

                foreach (var update in kvp.Value)
                {
                    var node = elements.FirstOrDefault(e => e["$id"]?.Value<string>() == update.nodeId);
                    if (node == null) continue;

                    var lineProp = node["defaultValues"]?["Dialogue Line"]?["$content"];
                    if (lineProp != null && lineProp.Value<string>() != update.line)
                    {
                        lineProp.Value<string>(update.line);
                        modified = true;
                    }

                    var charProp = node["defaultValues"]?["Character"]?["$content"];
                    if (charProp != null && charProp.Value<string>() != update.character)
                    {
                        charProp.Value<string>(update.character);
                        modified = true;
                    }
                }

                if (modified)
                {
                    data.jsonData = json.ToString(Newtonsoft.Json.Formatting.None);
                    updatedCount++;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Failed to update {dialogueName} during CSV import: {ex.Message}");
            }
        }

        Debug.Log($"Pipe-delimited CSV import complete. Updated {updatedCount} dialogues.");

        if (updatedCount > 0)
        {
            if (EditorUtility.DisplayDialog("Import Complete",
                $"Updated {updatedCount} dialogues.\nSave changes now?",
                "Save Now", "Later"))
            {
                SaveAllDialogues();
            }
        }
    }
}