using System;
using UnityEngine;

public class LocalizedTMPRegistrar : MonoBehaviour
{
    private void Awake()
    {
        foreach (var localizedTMP in FindObjectsOfType<LocalizedTMP>(includeInactive: true))
        {
            LocalizationManager.Instance.Register(localizedTMP);
        }
    }
}
