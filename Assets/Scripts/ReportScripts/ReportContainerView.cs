using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReportContainerView : MonoBehaviour
{
    [SerializeField] private List<ReportView> reports;
    //[SerializeField] private List<ReportSO> reportSOs;
    [SerializeField] private ReportView reportPrefab;
    [SerializeField] private Transform reportsParent;
    [SerializeField] private ReportDescriptionPanel reportDescriptionPanel;
    [SerializeField] private VerticalLayoutGroup reportsLayoutGroup;
    [SerializeField] private RectTransform reportsLayoutGroupRectTransform;
    private ReportView previousReport;

    [SerializeField] private List<ReportSO> robotReports;
    [SerializeField] private List<ReportSO> accessCodeReports;
    private bool hasRobotReports;
    private bool hasAccessCodeReports;
    public event Action onReportAdded = delegate { };

    private void OnReportSelected(ReportView report)
    {
        reportDescriptionPanel.Show();
        reportDescriptionPanel.SetDescription(report.GetDescription());
        reportDescriptionPanel.SetName(report.GetName());
    }

    public void AddReport(string name, string description)
    {
        ReportView report = ReportView.Create(reportPrefab, reportsParent, name.Translate(), description.Translate());
        reports.Add(report);
        report.OnReportSelected += OnReportSelected;
        previousReport = report;
        OnReportSelected(report);
        onReportAdded.Invoke();
    }

    [ContextMenu("Add Example Report")]
    public void AddExampleReport()
    {
        AddReport("Example Report", "This is an example description.");
    }

    internal void CompletePreviousReport()
    {
        if (!previousReport) return;

        previousReport.ChooseNew();
    }

    private async void OnEnable()
    {
        await UpdateReportPositions();
    }

    private async UniTask UpdateReportPositions()
    {
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

        reportsLayoutGroup.enabled = false;

        reportsLayoutGroup.CalculateLayoutInputVertical();

        LayoutRebuilder.ForceRebuildLayoutImmediate(reportsLayoutGroupRectTransform);

        reportsLayoutGroup.enabled = true;
    }

    [ContextMenu("Add Robot Reports")]
    public void AddRobotReports()
    {
        foreach (var report in robotReports)
        {
            AddReport(report.GetName(), report.GetDescription());
            hasRobotReports = true;
        }
    }

    [ContextMenu("Add Access Code Reports")]
    public void AddAccessCodeReports()
    {
        foreach(var report in accessCodeReports)
        {
            AddReport(report.GetName(), report.GetDescription());
            hasAccessCodeReports = true;
        }
    }

    public bool HasRobotReports() => hasRobotReports;
    public bool HasAccessCodeReports() => hasAccessCodeReports;
}