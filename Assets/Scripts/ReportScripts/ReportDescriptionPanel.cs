using TMPro;
using UnityEngine;

internal class ReportDescriptionPanel : Popup
{
    [SerializeField] private TextMeshProUGUI reportDescription;
    [SerializeField] private TextMeshProUGUI reportName;

    public void SetDescription(string description)
    {
        if (reportDescription == null)
        {
            reportDescription.text = "No Description";
            Debug.LogError("ReportDescription is not assigned in the ReportDescriptionPanel.");
            return;
        }
        reportDescription.text = description;
    }

    public void SetName(string name)
    {
        if (reportName == null)
        {
            reportName.text = "No Name";
            Debug.LogError("ReportName is not assigned in the ReportNamePanel.");
            return;
        }
        reportName.text = name;
    }
}