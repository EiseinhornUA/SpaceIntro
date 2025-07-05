using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Debuger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText;
    public void ShowDebugText(string text) => debugText.text = text;
}
