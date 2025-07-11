using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueReferences : MonoBehaviour
{
    [SerializeField] private DialogueView dialogueView;

    private void Awake()
    {
        //DialogueNode.view = dialogueView;
    }
}
