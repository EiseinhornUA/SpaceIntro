using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TaskAwaiter : MonoBehaviour
{
    [SerializeField] private UnityEvent onComplete;
    [SerializeField] private int secondsToWait = 60;
    private UniTaskCompletionSource completionSource = new();

    public void WaitUntilCompleted()
    {
        UniTask.WhenAny(WaitForSeconds(), WaitManualyTriggered()).ContinueWith(OnTimeTriggerComplete);
    }

    public void Triggered()
    {
        completionSource.TrySetResult();
    }

    private void OnTimeTriggerComplete(int taskNumber)
    {
        onComplete.Invoke();
    }

    private async UniTask WaitForSeconds()
    {
        await UniTask.WaitForSeconds(secondsToWait);
    }

    private async UniTask WaitManualyTriggered()
    {
        await completionSource.Task;
    }
}
