using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReadCarefullyPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI popUpOkText;
    [SerializeField] GameObject button;
    private const float timeUntilOkAllowed = 5;

    private void OnEnable()
    {
        Color disabledColor = new Color(0.784f, 0.784f, 0.784f, 1f);
        button.GetComponent<Image>().color = disabledColor;
        button.GetComponent<Button>().enabled = false;
        StartTimer();
    }

    private async UniTask ProcessTimer()
    {
        for (int i = 1; i < timeUntilOkAllowed + 1; i++)
        {
            await UniTask.WaitForSeconds(1);
            WriteTimeInOkText(i);
        }
        OnTimerExpired();
    }

    [ContextMenu("Start timer")]
    private void StartTimer() => ProcessTimer().Forget();

    private void OnTimerExpired()
    {
        button.GetComponent<Image>().color = Color.white;
        button.GetComponent<Button>().enabled = true;
    }

    private void WriteTimeInOkText(int seconds)
    {
        string text = (timeUntilOkAllowed - seconds == 0) ? "Ok".Translate() : $"({timeUntilOkAllowed - seconds})";
        popUpOkText.text = $"{text}";
    }
}
