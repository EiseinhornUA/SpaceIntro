using Cysharp.Threading.Tasks;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTimer : MonoBehaviour
{
    public int timeUsedForDialogue = 0;
    private float floatTimeUsedForDialogue = 0;
    private bool isTimerActive = false;

    [SerializeField] private DatabaseManager databaseManager;
    [SerializeField] private DialogueManager dialogueManager;

    private void Update()
    {
        if (isTimerActive)
        {
            floatTimeUsedForDialogue += Time.deltaTime;
            timeUsedForDialogue = (int)floatTimeUsedForDialogue;
        }
    }

    public void ActivateTimer()
    {
        isTimerActive = true;
        floatTimeUsedForDialogue = 0;
    }

    public void SaveTime()
    {
        isTimerActive = false;
        databaseManager.SaveDialogueTime(timeUsedForDialogue, dialogueManager.GetDialogueName());
    }
}
