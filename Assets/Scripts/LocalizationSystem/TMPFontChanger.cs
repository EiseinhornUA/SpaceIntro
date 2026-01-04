using UnityEngine;
using UnityEditor;
using TMPro;

public class TMPFontChanger : EditorWindow
{
    private TMP_FontAsset newFont;

    [MenuItem("Tools/TMP Font Changer")]
    public static void ShowWindow()
    {
        GetWindow<TMPFontChanger>("TMP Font Changer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Change TMP Fonts in Scene", EditorStyles.boldLabel);
        newFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New Font", newFont, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Change All TMP Fonts"))
        {
            if (newFont != null)
            {
                ChangeAllFonts();
            }
            else
            {
                Debug.LogWarning("Please assign a new TMP Font!");
            }
        }
    }

    private void ChangeAllFonts()
    {
        // Find all TextMeshProUGUI components in the scene
        TextMeshProUGUI[] tmpComponents = FindObjectsOfType<TextMeshProUGUI>(true);

        int count = 0;
        foreach (TextMeshProUGUI tmp in tmpComponents)
        {
            Undo.RecordObject(tmp, "Change TMP Font"); // Allow undo
            tmp.font = newFont;
            count++;
        }

        Debug.Log($"Changed font on {count} TextMeshProUGUI components.");
    }
}

