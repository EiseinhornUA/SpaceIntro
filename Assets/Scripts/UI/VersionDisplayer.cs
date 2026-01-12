using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class VersionDisplayer : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TextMeshProUGUI>().text = "version: " + Application.version;
    }
}
