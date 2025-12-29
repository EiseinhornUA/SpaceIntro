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

                        // Make the text field editable and capture changes
                        string newText = EditorGUILayout.TextArea(dialogueLine);
                        if (newText != dialogueLine)
                        {
                            SetValueSafe(node["defaultValues"]?["Dialogue Line"], "$content", newText);
                        }
                    }

                    if (nodeType == "DialogueChoiceNode" || nodeType == "DialogueIterativeNode")
                    {
                        string dialogueLine = GetValueSafe(node["defaultValues"]?["Dialogue Line"], "$content") ?? "";
                        EditorGUILayout.LabelField(nodeType, EditorStyles.boldLabel);

                        string newDialogueLine = EditorGUILayout.TextArea(dialogueLine);
                        if (newDialogueLine != dialogueLine)
                        {
                            SetValueSafe(node["defaultValues"]?["Dialogue Line"], "$content", newDialogueLine);
                        }

                        for (int i = 1; i <= 4; i++)
                        {
                            string key = $"Text{i}";
                            string text = GetValueSafe(node["defaultValues"]?[key], "$content") ?? "";
                            if (!string.IsNullOrEmpty(text))
                            {
                                EditorGUILayout.LabelField($"Choice {i}:", EditorStyles.miniBoldLabel);

                                string newText = EditorGUILayout.TextArea(text);
                                if (newText != text)
                                {
                                    SetValueSafe(node["defaultValues"]?[key], "$content", newText);
                                }
                            }
                        }
                    }
                }

                if (EditorGUI.EndChangeCheck())
                {
                    data.jsonData = json.ToString();
                    dialogues[kvp.Key] = data;
                }

                if (GUILayout.Button($"Save {data.name}"))
                    SaveDialogue(data);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        EditorGUILayout.EndScrollView();
    }

    private void SetValueSafe(JToken token, string propertyName, string value)
    {
        if (token != null && token is JObject obj)
        {
            obj[propertyName] = value;
        }
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