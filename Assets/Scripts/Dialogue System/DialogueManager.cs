using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueView dialogueView;
    private DialoguePrefab dialogueInstance;
    private List<Decision> dialogueDecisions;
    private Decision selectedDecision;

    public void StartDialogue(DialoguePrefab dialoguePrefab)
    {
        if (dialogueInstance) 
            Destroy(dialogueInstance.gameObject);
        dialogueView.Show();
        dialogueInstance = GameObject.Instantiate(dialoguePrefab);
    }

    public async UniTask WaitForDialogueEnd()
    {
        await dialogueView.WaitForHide();
    }

    internal void SetDecisions(List<Decision> decisions) => dialogueDecisions = decisions;
    internal List<Decision> GetDecisions() => dialogueDecisions;

    internal void SetSelectedDecision(Decision decision) => selectedDecision = decision;
    internal Decision GetSelectedDecision() => selectedDecision;
}
