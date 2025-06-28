using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialogueView))]
public class DialogueManager : MonoBehaviour
{
    private DialogueView dialogueDisplayer;

    private void Start()
    {
        dialogueDisplayer = GetComponent<DialogueView>();
    }
}
