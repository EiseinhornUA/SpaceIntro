using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textTMP;

    private void Start()
    {
        textTMP.text = "Version " + Application.version;
    }
}
