using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalView : MonoBehaviour
{
    [SerializeField] private List<TaskView> tasks;
    [SerializeField] private TaskView taskPrefab;
    [SerializeField] private Transform tasksParent;
    [SerializeField] private TaskDescriptionPanel taskDescriptionPanel;
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
}
