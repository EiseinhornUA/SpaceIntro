using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePrefab : MonoBehaviour
{
    public void StartDialogue()
    {
        gameObject.SetActive(true);
    }

    public void StopDialogue()
    {
        gameObject.SetActive(false);
    }
}
