using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue")]
public class DialogueSO : ScriptableObject
{
    public List<DialogueItem> nodes;
}

[System.Serializable]
public class DialogueItem
{
    public DialogueCharacter speaker;
    [TextArea] public string text;
    public List<DialogueChoice> choices;
}

[System.Serializable]
public class DialogueChoice
{
    [TextArea] public string choiceText;
    public DialogueItem nextNode;
}
