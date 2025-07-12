using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueView dialogueView;
    private DialoguePrefab dialogueInstance;

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
}
