using TMPro;
using UnityEngine;

public class TaskView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI taskText;
    
    public void SetTaskText(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        taskText.text = text;
    }
}