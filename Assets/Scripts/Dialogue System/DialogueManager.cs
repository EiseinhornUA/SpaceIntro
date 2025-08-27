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
    [SerializeField] private List<DialoguePrefab> dialoguePrefabs;
    private DialoguePrefab dialogueInstance;
    private List<Decision> dialogueDecisions;
    private Decision selectedDecision;
    public Action onDialogueStart = delegate {};



    public void StartDialogue(DialoguePrefab dialoguePrefab)
    {
        dialogueView.Show();

        dialogueInstance = dialoguePrefabs.Find(dialogue => dialogue.name == dialoguePrefab.name);
        dialogueInstance.StartDialogue();

        onDialogueStart?.Invoke();
    }

    public async UniTask WaitForDialogueEnd()
    {
        await dialogueView.WaitForHide();
        dialogueInstance.StopDialogue();
    }

    internal void SetDecisions(List<Decision> decisions) => dialogueDecisions = decisions;
    internal List<Decision> GetDecisions() => dialogueDecisions;

    internal void SetSelectedDecision(Decision decision) => selectedDecision = decision;
    internal Decision GetSelectedDecision() => selectedDecision;
}
