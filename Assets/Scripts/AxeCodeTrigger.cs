using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeCodeTrigger : MonoBehaviour
{
    private const int numberOfTriesUntilTriggerActivates = 5;
    [SerializeField] private NumPad numPad;
    [SerializeField] private int errorCounter;
    [SerializeField] private BoxCollider2D trigger;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialoguePrefab firstAxeHint;
    private bool hintTriggered = false;

    private void Start()
    {
        trigger.enabled = false;
    }

    public void OnErrorInNumpad()
    {
        errorCounter++;
        if (errorCounter >= numberOfTriesUntilTriggerActivates)
            trigger.enabled = true;
    }

    public void TryToStartHint()
    {
        if (errorCounter >= numberOfTriesUntilTriggerActivates && hintTriggered == false)
        {
            dialogueManager.StartDialogue(firstAxeHint);
            hintTriggered = true;
        }
    }
}
