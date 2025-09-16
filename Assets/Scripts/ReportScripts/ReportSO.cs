using UnityEngine;


[CreateAssetMenu(fileName = "Report", menuName = "ScriptableObjects/Report")]
public class ReportSO : ScriptableObject
{
    [SerializeField] private string reportName;
    [TextArea(3, 15)]
    [SerializeField] private string reportDescription;

    public string GetName() => reportName;
    public string GetDescription() => reportDescription;
}