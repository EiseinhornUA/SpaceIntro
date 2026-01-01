using System;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEngine.UI;

public class LocalizatorView : MonoBehaviour
{
    [SerializeField] private List<ButtonTranslationPair> buttonTranslationPairList;

    private void Awake()
    {
        foreach (var pair in buttonTranslationPairList)
        {
            var button = pair.button;
            var translation = pair.translation;
            button.onClick.AddListener(() => LocalizationManager.Instance.ChangeLanguage(translation));
        }
    }
}

[Serializable]
public class ButtonTranslationPair
{
    public Button button; 
    public TranslationSO translation;
}