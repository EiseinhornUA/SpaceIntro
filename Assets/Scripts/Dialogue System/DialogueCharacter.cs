using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueCharacter", menuName = "ScriptableObjects/Dialogue/DialogueCharacter")]
public class DialogueCharacter : ScriptableObject
{
    [SerializeField] private Sprite portrait;
    public string GetName() => name;
    public Sprite GetPortrait() => portrait;
    public void SetPortrait(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            throw new System.Exception("Portrait path is not valid.");
        }
        portrait = SpriteLoader.LoadSpriteFromDisk(path);
    }
}

