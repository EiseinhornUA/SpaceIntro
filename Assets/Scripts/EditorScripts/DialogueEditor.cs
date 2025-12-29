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
    private string dialoguesPath = "Assets/Dialogues"; // ← change if needed

    [System.Serializable]
    private class DialogueData
    {
        public string name;
        public string jsonData;
        public JObject jsonDataParsed;
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
                    jsonDataParsed = JObject.Parse(json),
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
            LoadDialogues();

        if (GUILayout.Button("Save All"))
            SaveAllDialogues();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Export Combined JSON"))
            ExportCombinedJson();

        if (GUILayout.Button("Import Combined JSON"))
            ImportCombinedJson();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Export to Pipe CSV (|)"))
            ExportToPipeCsv();

        if (GUILayout.Button("Import from Pipe CSV (|)"))
            ImportFromPipeCsv();
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

                EditorGUI.BeginChangeCheck();
                var json = data.jsonDataParsed;

                JArray elements = json["graph"]?["elements"] as JArray;
                if (elements == null) continue;

                foreach (JObject node in elements)
                {
                    string nodeType = node["$type"]?.Value<string>() ?? "";

                    if (nodeType == "DialogueNode")
                    {
                        string dialogueLine = GetValueSafe(node["defaultValues"]?["Dialogue Line"], "$content") ?? "";
                        EditorGUILayout.LabelField(nodeType, EditorStyles.boldLabel);

                        EditorGUILayout.LabelField("Original Text:", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(dialogueLine);

                        EditorGUILayout.TextArea(dialogueLine);
                    }
                    if (nodeType == "DialogueChoiceNode")
                    {
                        string dialogueLine = GetValueSafe(node["defaultValues"]?["Dialogue Line"], "$content") ?? "";
                        EditorGUILayout.LabelField(nodeType, EditorStyles.boldLabel);
                        EditorGUILayout.LabelField("Original Text:", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(dialogueLine);
                        EditorGUILayout.TextArea(dialogueLine);
                        string text1 = GetValueSafe(node["defaultValues"]?["Dialogue Line"], "$content") ?? "";
                        for (int i = 1; i <= 4; i++)
                        {
                            string key = $"Text{i}";
                            string text = GetValueSafe(node["defaultValues"]?[key], "$content") ?? "";
                            if (!string.IsNullOrEmpty(text))
                            {
                                EditorGUILayout.LabelField($"Choice {i}:");
                                EditorGUILayout.LabelField("Original Text:", EditorStyles.boldLabel);
                                EditorGUILayout.LabelField(dialogueLine);
                                EditorGUILayout.TextArea(text);
                            }
                        }
                    }
                    if (nodeType == "DialogueIterativeNode")
                    {
                        string dialogueLine = GetValueSafe(node["defaultValues"]?["Dialogue Line"], "$content") ?? "";
                        EditorGUILayout.LabelField(nodeType, EditorStyles.boldLabel);
                        EditorGUILayout.LabelField("Original Text:", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(dialogueLine);
                        EditorGUILayout.TextArea(dialogueLine);
                        string text1 = GetValueSafe(node["defaultValues"]?["Dialogue Line"], "$content") ?? "";
                        for (int i = 1; i <= 4; i++)
                        {
                            string key = $"Text{i}";
                            string text = GetValueSafe(node["defaultValues"]?[key], "$content") ?? "";
                            if (!string.IsNullOrEmpty(text))
                            {
                                EditorGUILayout.LabelField($"Choice {i}:");
                                EditorGUILayout.LabelField("Original Text:", EditorStyles.boldLabel);
                                EditorGUILayout.LabelField(dialogueLine);
                                EditorGUILayout.TextArea(text);
                            }
                        }
                    }


                }

                //data.jsonData = EditorGUILayout.TextArea(data.jsonData, GUILayout.Height(100));
                if (EditorGUI.EndChangeCheck())
                    dialogues[kvp.Key] = data;

                if (GUILayout.Button($"Save {data.name}"))
                    SaveDialogue(data);

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
            SaveDialogue(data);

        Debug.Log("All dialogues saved");
    }

    // ──────────────────────────────────────────────────────────────
    // Combined JSON Export / Import
    // ──────────────────────────────────────────────────────────────

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

            int imported = 0, notFound = 0;

            foreach (var prop in combined.Properties())
            {
                string name = prop.Name;
                if (dialogues.ContainsKey(name))
                {
                    dialogues[name].jsonData = prop.Value.ToString(Newtonsoft.Json.Formatting.None);
                    imported++;
                }
                else
                {
                    Debug.LogWarning($"Dialogue '{name}' not found. Skipping.");
                    notFound++;
                }
            }

            Debug.Log($"Import done: {imported} imported, {notFound} not found");

            if (imported > 0 && EditorUtility.DisplayDialog("Import Complete",
                $"Imported {imported} dialogues.\nSave now?", "Save", "Later"))
            {
                SaveAllDialogues();
            }
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Import Failed", e.Message, "OK");
            Debug.LogError(e);
        }
    }

    // ──────────────────────────────────────────────────────────────
    // Pipe-delimited CSV Export (no Character column)
    // ──────────────────────────────────────────────────────────────

    private void ExportToPipeCsv()
    {
        string path = EditorUtility.SaveFilePanel("Export to Pipe CSV", "", "dialogues_pipe.csv", "csv");
        if (string.IsNullOrEmpty(path)) return;

        using (var writer = new StreamWriter(path))
        {
            writer.WriteLine("DialogueName|NodeId|NodeType|Field|Value");

            foreach (var kvp in dialogues)
            {
                string dlgName = kvp.Key;
                try
                {
                    JObject json = JObject.Parse(kvp.Value.jsonData);
                    JArray elements = json["graph"]?["elements"] as JArray;
                    if (elements == null) continue;

                    foreach (JObject node in elements)
                    {
                        string nodeType = node["$type"]?.Value<string>() ?? "";
                        if (!nodeType.Contains("Dialogue")) continue;

                        string nodeId = node["$id"]?.Value<string>() ?? "";

                        var defaults = node["defaultValues"] as JObject;
                        if (defaults == null) continue;

                        // Main dialogue line (always use "Dialogue Line" with space)
                        string mainLine = GetValueSafe(defaults["Dialogue Line"], "$content")
                                       ?? GetValueSafe(defaults["Dialogue Line"], null)
                                       ?? GetValueSafe(defaults["DialogueLine"], "$content")
                                       ?? GetValueSafe(defaults["DialogueLine"], null) ?? "";

                        if (!string.IsNullOrEmpty(mainLine))
                        {
                            string escaped = EscapeCsvValue(mainLine);
                            writer.WriteLine($"{dlgName}|{nodeId}|{nodeType}|Dialogue Line|{escaped}");
                        }

                        // Choice/iterative texts
                        for (int i = 1; i <= 8; i++)
                        {
                            string key = $"Text{i}";
                            string text = GetValueSafe(defaults[key], "$content")
                                       ?? GetValueSafe(defaults[key], null) ?? "";

                            if (!string.IsNullOrEmpty(text))
                            {
                                string escaped = EscapeCsvValue(text);
                                writer.WriteLine($"{dlgName}|{nodeId}|{nodeType}|{key}|{escaped}");
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"Export failed for {dlgName}: {ex.Message}");
                }
            }
        }

        Debug.Log($"CSV exported (without Character): {path}");
    }

    // ──────────────────────────────────────────────────────────────
    // Pipe-delimited CSV Import - Robust version
    // ──────────────────────────────────────────────────────────────

    private void ImportFromPipeCsv()
    {
        string path = EditorUtility.OpenFilePanel("Import Pipe CSV", "", "csv");
        if (string.IsNullOrEmpty(path)) return;

        var updates = new Dictionary<string, List<(string nodeId, string field, string value)>>();

        string[] lines = File.ReadAllLines(path);
        int parsed = 0, skipped = 0;

        for (int i = 1; i < lines.Length; i++)
        {
            string raw = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(raw)) continue;

            var parts = SplitWithEscape(raw, '|');
            if (parts.Count < 5)
            {
                skipped++;
                continue;
            }

            string dlgName = parts[0].Trim();
            string nodeId = parts[1].Trim();
            string field = parts[3].Trim();

            // Take only the value column (ignore anything after)
            string rawValue = string.Join("|", parts.GetRange(4, parts.Count - 4));
            string value = UnescapeCsvValue(rawValue.TrimEnd('|', ' ', '-').Trim());

            if (string.IsNullOrWhiteSpace(value)) continue;

            if (!updates.TryGetValue(dlgName, out var list))
            {
                list = new List<(string, string, string)>();
                updates[dlgName] = list;
            }

            list.Add((nodeId, field, value));
            parsed++;
        }

        Debug.Log($"CSV parsed: {parsed} valid lines, {skipped} skipped");

        int updatedCount = 0;

        foreach (var kvp in updates)
        {
            string dlgName = kvp.Key;
            if (!dialogues.TryGetValue(dlgName, out var data))
            {
                Debug.LogError($"Dialogue '{dlgName}' not found. Skipping updates.");
                continue;
            }

            try
            {
                JObject json = JObject.Parse(data.jsonData);
                JArray elements = json["graph"]?["elements"] as JArray;
                if (elements == null) continue;

                bool modified = false;

                foreach (var upd in kvp.Value)
                {
                    string targetField = upd.field;

                    var node = elements.FirstOrDefault(e => e["$id"]?.Value<string>() == upd.nodeId);
                    if (node == null) continue;

                    var defaults = node["defaultValues"] as JObject;
                    if (defaults == null) continue;

                    JToken token = defaults[targetField];
                    if (token == null)
                    {
                        Debug.LogWarning($"Field '{targetField}' not found in node ({upd.nodeId}) of '{dlgName}'");
                        Debug.LogError(defaults.ToString());
                        continue;
                    }

                    string currentValue = null;

                    // Case 1: Structured object with $content
                    if (token is JObject obj && obj["$content"] != null)
                    {
                        currentValue = obj["$content"]?.Value<string>() ?? obj["$content"]?.ToString();

                        if (currentValue != upd.value)
                        {
                            Debug.Log($"SAFE UPDATE: {dlgName} → node {upd.nodeId} → {targetField} → '{currentValue}' → '{upd.value}'");

                            // IMPORTANT: Only update $content, keep $type and other properties!
                            obj["$content"] = upd.value;
                            modified = true;
                        }
                    }
                    // Case 2: Direct string value
                    else if (token.Type == JTokenType.String)
                    {
                        currentValue = token.Value<string>();

                        if (currentValue != upd.value)
                        {
                            Debug.Log($"UPDATE direct string: {dlgName} → node {upd.nodeId} → {targetField} → '{currentValue}' → '{upd.value}'");
                            defaults[targetField] = upd.value;
                            modified = true;
                        }
                    }
                    // Case 3: Numeric value (e.g. Character as int)
                    else if (token.Type == JTokenType.Integer || token.Type == JTokenType.Float)
                    {
                        if (int.TryParse(upd.value, out int newInt))
                        {
                            int oldInt = token.Value<int>();
                            if (oldInt != newInt)
                            {
                                Debug.Log($"UPDATE numeric: {dlgName} → node {upd.nodeId} → {targetField} → {oldInt} → {newInt}");
                                defaults[targetField] = newInt;
                                modified = true;
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"Cannot parse '{upd.value}' as int for numeric field {targetField} in {dlgName}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Unsupported token type for field {targetField} in node {upd.nodeId}: {token.Type}");
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
                Debug.LogWarning($"Failed to process {dlgName}: {ex.Message}");
            }
        }

        Debug.Log($"Import complete. Updated {updatedCount} dialogues.");

        if (updatedCount > 0)
        {
            if (EditorUtility.DisplayDialog("Import Done",
                $"Modified {updatedCount} dialogues.\nSave now?",
                "Save", "Later"))
            {
                SaveAllDialogues();
            }
        }
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static string GetValueSafe(JToken token, string subKey)
    {
        if (token == null) return null;
        if (subKey != null && token is JObject obj && obj[subKey] != null)
            return obj[subKey].Value<string>();
        if (token.Type == JTokenType.String)
            return token.Value<string>();
        return null;
    }

    private static string EscapeCsvValue(string value)
    {
        if (string.IsNullOrEmpty(value)) return "";
        return value.Replace("|", "\\|").Replace("\r\n", "\\n").Replace("\n", "\\n");
    }

    private static List<string> SplitWithEscape(string input, char delimiter)
    {
        var result = new List<string>();
        var current = new System.Text.StringBuilder();
        bool escaped = false;

        foreach (char c in input)
        {
            if (escaped)
            {
                if (c == delimiter) current.Append(delimiter);
                else if (c == 'n') current.Append('\n');
                else { current.Append('\\'); current.Append(c); }
                escaped = false;
            }
            else if (c == '\\')
            {
                escaped = true;
            }
            else if (c == delimiter)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        if (current.Length > 0)
            result.Add(current.ToString());

        return result;
    }

    private static string UnescapeCsvValue(string value)
    {
        return value.Replace("\\|", "|").Replace("\\n", "\n");
    }
}