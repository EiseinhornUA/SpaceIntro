using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedTMP : MonoBehaviour
{
    public string key;
    private TextMeshProUGUI text;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        LocalizationManager.Instance.Register(this);
    }

    public void Apply(TranslationSO so)
    {
        text.text = so.Get(key);
    }
}
