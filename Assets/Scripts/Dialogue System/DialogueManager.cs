using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    private DialogueView dialogueView;
    private DialoguePrefab dialogueInstance;

    private void Start()
    {
        dialogueView = GameObject.FindObjectOfType<DialogueView>(includeInactive: true);
    }

    public void StartDialogue(DialoguePrefab dialoguePrefab)
    {
        if (dialogueInstance) 
            Destroy(dialogueInstance.gameObject);
        dialogueView.Show();
        dialogueInstance = GameObject.Instantiate(dialoguePrefab);
    }
}
