using UnityEditor;
using UnityEngine;

public class ScriptableObjectSerializedTextViewer : EditorWindow
{
    [MenuItem("Tools/ScriptableObject Serialized Text Viewer")]
    static void Open() => GetWindow<ScriptableObjectSerializedTextViewer>();

    Vector2 scroll;
    string findText = "";
    string replaceText = "";
    bool exportNames = false;

    void OnGUI()
    {
        DrawFindReplaceBar();

        scroll = EditorGUILayout.BeginScrollView(scroll);

        foreach (var obj in Selection.objects)
        {
            if (obj is not ScriptableObject so) continue;

            var serialized = new SerializedObject(so);
            var prop = serialized.GetIterator();

            EditorGUILayout.LabelField(so.name, EditorStyles.boldLabel);
            Undo.RecordObject(so, "Edit ScriptableObject Text");

            while (prop.NextVisible(true))
            {
                if (prop.propertyType != SerializedPropertyType.String)
                    continue;

                EditorGUILayout.LabelField(prop.displayName);

                float width = position.width - 40f;
                float height = Mathf.Max(
                    20f,
                    EditorStyles.textArea.CalcHeight(
                        new GUIContent(prop.stringValue),
                        width
                    )
                );

                EditorGUI.BeginChangeCheck();
                string newValue = EditorGUILayout.TextArea(
                    prop.stringValue,
                    GUILayout.Height(height)
                );

                if (EditorGUI.EndChangeCheck())
                    prop.stringValue = newValue;

                EditorGUILayout.Space(4);
            }

            if (serialized.ApplyModifiedProperties())
                EditorUtility.SetDirty(so);

            EditorGUILayout.Space(8);
        }

        EditorGUILayout.EndScrollView();

        exportNames = EditorGUILayout.ToggleLeft(
            "Export ScriptableObject names",
            exportNames
        );

        if (GUILayout.Button("Save All"))
            AssetDatabase.SaveAssets();

        if (GUILayout.Button("Export | CSV"))
            ExportPipeCsv();
    }

    void DrawFindReplaceBar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

        findText = EditorGUILayout.TextField("Find", findText);
        replaceText = EditorGUILayout.TextField("Replace", replaceText);

        GUI.enabled = !string.IsNullOrEmpty(findText);

        if (GUILayout.Button("Replace All", GUILayout.Width(100)))
            ReplaceAll();

        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
    }

    void ReplaceAll()
    {
        foreach (var obj in Selection.objects)
        {
            if (obj is not ScriptableObject so) continue;

            Undo.RecordObject(so, "Find & Replace Text");

            var serialized = new SerializedObject(so);
            var prop = serialized.GetIterator();

            while (prop.NextVisible(true))
            {
                if (prop.propertyType != SerializedPropertyType.String)
                    continue;

                if (!string.IsNullOrEmpty(prop.stringValue) &&
                    prop.stringValue.Contains(findText))
                {
                    prop.stringValue =
                        prop.stringValue.Replace(findText, replaceText);
                }
            }

            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(so);
        }

        AssetDatabase.SaveAssets();
    }

    void ExportPipeCsv()
    {
        string path = EditorUtility.SaveFilePanel(
            "Export Pipe CSV",
            Application.dataPath,
            "TextValues",
            "csv"
        );

        if (string.IsNullOrEmpty(path))
            return;

        using var writer = new System.IO.StreamWriter(
            path,
            false,
            System.Text.Encoding.UTF8
        );

        foreach (var obj in Selection.objects)
        {
            if (obj is not ScriptableObject so) continue;

            if (exportNames)
                writer.WriteLine(Escape(so.name));

            var serialized = new SerializedObject(so);
            var prop = serialized.GetIterator();

            while (prop.NextVisible(true))
            {
                if (prop.propertyType != SerializedPropertyType.String)
                    continue;

                writer.WriteLine(Escape(prop.stringValue));
            }
        }

        AssetDatabase.Refresh();
    }

    string Escape(string s)
    {
        if (string.IsNullOrEmpty(s))
            return "\"\"";

        return "\"" + s.Replace("\"", "\"\"") + "\"";
    }
}

