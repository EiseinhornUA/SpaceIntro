using TMPro;
using UnityEngine;

internal class TaskDescriptionPanel : Popup
{
    [SerializeField] private LocalizedTMP taskDescription;

    public void SetDescription(string description)
    {
        if (taskDescription == null)
        {
            taskDescription.SetTextAndKey("No Description");
            Debug.LogError("TaskDescription is not assigned in the TaskDescriptionPanel.");
            return;
        }
        taskDescription.SetTextAndKey(description);
    }
}