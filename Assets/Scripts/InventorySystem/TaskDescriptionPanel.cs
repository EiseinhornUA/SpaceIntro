using TMPro;
using UnityEngine;

internal class TaskDescriptionPanel : Popup
{
    [SerializeField] private TextMeshProUGUI taskDescription;

    public void SetDescription(string description)
    {
        if (taskDescription == null)
        {
            taskDescription.text = "No Description";
            Debug.LogError("TaskDescription is not assigned in the TaskDescriptionPanel.");
            return;
        }
        taskDescription.text = description;
    }
}