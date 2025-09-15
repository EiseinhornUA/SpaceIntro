using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reportNameText;
    [SerializeField] private Image reportBackground;
    [SerializeField] private Button button;
    private string description;

    public event Action<ReportView> OnReportSelected = delegate { };

    private void Awake()
    {
        button.onClick.AddListener(OnReportClicked);
    }

    public void ChooseNew()
    {
        reportNameText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        reportBackground.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }

    public static ReportView Create(ReportView reportPrefab, Transform parent, string reportName, string description)
    {
        ReportView report = Instantiate(reportPrefab, parent);
        report.reportNameText.text = reportName;
        report.description = description;
        return report;
    }

    private void OnReportClicked()
    {
        OnReportSelected(this);
    }
    
    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnReportClicked);
    }

    public string GetDescription() => description;
    public string GetName() => reportNameText.text;
}