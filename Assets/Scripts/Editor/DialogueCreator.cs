using UnityEditor;
using UnityEngine;
using Unity.VisualScripting;
using System.IO;

public class DialogueCreator : EditorWindow
{
    private string assetName = "NewDialogue";

    private static readonly string basePath = "Assets/Dialogues/Dialogues";
    private static ScriptGraphAsset templateGraph;
    private static GameObject dialoguePrefab;

    [MenuItem("Assets/Create/Dialogue Prefab with Graph", false, 10)]
    private static void Init()
    {
        string graphPath = Path.Combine(basePath, "DialogueTemplate.asset");
        string prefabPath = Path.Combine(basePath, "DialogueTemplate.prefab");

        templateGraph = AssetDatabase.LoadAssetAtPath<ScriptGraphAsset>(graphPath);
        dialoguePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        Debug.Log($"Loaded template graph: {templateGraph}, prefab: {dialoguePrefab}");

        if (templateGraph == null || dialoguePrefab == null)
        {
            Debug.LogError("Missing DialogueTemplate.asset or DialoguePrefab.prefab in Dialogues folder.");
            return;
        }

        var window = CreateInstance<DialogueCreator>();
        window.titleContent = new GUIContent("New Dialogue");
        Vector2 size = new Vector2(300, 100);
        window.position = new Rect(
            (Screen.currentResolution.width - size.x) / 2f,
            (Screen.currentResolution.height - size.y) / 2f,
            size.x, size.y);
        window.ShowPopup();
    }

    private void OnGUI()
    {
        GUILayout.Label("Enter Dialogue Name", EditorStyles.boldLabel);
        assetName = EditorGUILayout.TextField("Name", assetName);

        if (GUILayout.Button("Create"))
        {
            CreateAssets();
            Close();
        }
    }

    private void CreateAssets()
    {
        string graphPath = Path.Combine(basePath, assetName + ".asset");
        string prefabPath = Path.Combine(basePath, assetName + ".prefab");

        // Duplicate graph
        var newGraph = Instantiate(templateGraph);
        newGraph.name = assetName;
        AssetDatabase.CreateAsset(newGraph, graphPath);
        AssetDatabase.SaveAssets();

        // Create prefab variant
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(dialoguePrefab);
        instance.name = assetName;

        var machine = instance.GetComponent<ScriptMachine>();
        if (machine != null)
            machine.nest.SwitchToMacro(newGraph);

        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        DestroyImmediate(instance);

        Selection.activeObject = newGraph;
        EditorApplication.ExecuteMenuItem("Window/Visual Scripting/Graph");
    }
}
