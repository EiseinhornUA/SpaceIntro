using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalView : MonoBehaviour
{
    [SerializeField] private List<TaskView> tasks;
    [SerializeField] private TaskView taskPrefab;
    [SerializeField] private Transform tasksParent;
    [SerializeField] private TaskDescriptionPanel taskDescriptionPanel;
    [SerializeField] private VerticalLayoutGroup tasksLayoutGroup;
    [SerializeField] private RectTransform tasksLayoutGroupRectTransform;
    private TaskView previousTask;

    private void OnTaskSelected(TaskView task)
    {
        taskDescriptionPanel.Show();
        taskDescriptionPanel.SetDescription(task.GetDescription());
    }

    public void AddTask(string name, string description)
    {
        TaskView task = TaskView.Create(taskPrefab, tasksParent, name, description);
        tasks.Add(task);
        task.OnTaskSelected += OnTaskSelected;
        previousTask = task;
        OnTaskSelected(task);
    }

    [ContextMenu("Add Example Task")]
    public void AddExampleTask()
    {
        AddTask("Example Task", "This is an example description.");
    }

    internal void CompletePreviousTask()
    {
        if (!previousTask) return;

        previousTask.Complete();
    }

    //private async UniTask UpdateTaskPositions()
    //{
    //    tasksLayoutGroup.enabled = false;
    //    await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
    //    tasksLayoutGroup.enabled = true;
    //}

    private async void OnEnable()
    {
        await UpdateTaskPositions();
    }

    private async UniTask UpdateTaskPositions()
    {
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

        tasksLayoutGroup.enabled = false;

        tasksLayoutGroup.CalculateLayoutInputVertical();

        LayoutRebuilder.ForceRebuildLayoutImmediate(tasksLayoutGroupRectTransform);

        tasksLayoutGroup.enabled = true;
    }
}
