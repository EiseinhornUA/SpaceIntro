using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedTMP : MonoBehaviour
{
    [field: SerializeField] public string key { get; private set; }
    private TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        LocalizationManager.Instance.Register(this);
    }

    public void Apply(LocalizationTableSO table, SystemLanguage language)
    {
        text.text = table.Get(key, language);
    }
}
