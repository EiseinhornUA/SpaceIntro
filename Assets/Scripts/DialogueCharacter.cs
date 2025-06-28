using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueCharacter", menuName = "ScriptableObjects/Dialogue/DialogueCharacter")]
public class DialogueCharacter : ScriptableObject
{
    [SerializeField] private Sprite portrait;
    public string GetName() => name;
    public Sprite GetPortrait() => portrait;
}

