using UnityEngine;
using UnityEditor;
using TMPro;

public class TMPFontChanger : EditorWindow
{
    TMP_FontAsset newFont;

    [MenuItem("Tools/TMP Font Changer (Scene + Prefabs)")]
    static void Open() => GetWindow<TMPFontChanger>("TMP Font Changer");

    void OnGUI()
    {
        EditorGUILayout.LabelField("Change TMP Fonts Everywhere", EditorStyles.boldLabel);
        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField(
            "New TMP Font", newFont, typeof(TMP_FontAsset), false);

        GUILayout.Space(10);

        if (GUILayout.Button("Change Fonts in Scene + Prefabs"))
        {
            if (newFont == null)
            {
                Debug.LogWarning("Assign a TMP Font Asset first.");
                return;
            }

            ChangeSceneFonts();
            ChangePrefabFonts();
            AssetDatabase.SaveAssets();
            Debug.Log("TMP font replacement finished.");
        }
    }

    void ChangeSceneFonts()
    {
        var texts = Object.FindObjectsOfType<TextMeshProUGUI>(true);

        foreach (var tmp in texts)
        {
            Undo.RecordObject(tmp, "Change TMP Font");
            tmp.font = newFont;
            EditorUtility.SetDirty(tmp);
        }

        Debug.Log($"Scene: updated {texts.Length} TMP components.");
    }

    void ChangePrefabFonts()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        int count = 0;

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = PrefabUtility.LoadPrefabContents(path);

            bool modified = false;
            var tmps = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);

            foreach (var tmp in tmps)
            {
                tmp.font = newFont;
                modified = true;
                count++;
            }

            if (modified)
                PrefabUtility.SaveAsPrefabAsset(prefab, path);

            PrefabUtility.UnloadPrefabContents(prefab);
        }

        Debug.Log($"Prefabs: updated {count} TMP components.");
    }
}

