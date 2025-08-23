using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI taskNameText;
    [SerializeField] private Button button;
    private string description;

    public event Action<TaskView> OnTaskSelected = delegate { };

    private void Awake()
    {
        button.onClick.AddListener(OnTaskClicked);
    }

    public void Complete()
    {
        taskNameText.text = "<s>" + taskNameText.text + "</s>";
    }

    public static TaskView Create(TaskView taskPrefab, Transform parent, string taskName, string description)
    {
        TaskView task = Instantiate(taskPrefab, parent);
        task.taskNameText.text = taskName;
        task.description = description;
        return task;
    }

    private void OnTaskClicked()
    {
        OnTaskSelected(this);
    }
    
    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnTaskClicked);
    }

    public string GetDescription() => description;
}