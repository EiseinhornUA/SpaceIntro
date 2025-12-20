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
}
