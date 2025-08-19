using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class DebugMenu : Popup
{
    [SerializeField] private Volume postProcessingVolume;
    [SerializeField] private Toggle togglePostProcessing;
    [SerializeField] private Toggle toggleBloom;
    [SerializeField] private Toggle toggleTonemapping;
    [SerializeField] private Toggle toggleColorAjustments;

    private void Start()
    {
        togglePostProcessing.onValueChanged.AddListener(TogglePostProcessing);
        toggleBloom.onValueChanged.AddListener(ToggleBloom);
        toggleTonemapping.onValueChanged.AddListener(ToggleTonemapping);
        toggleColorAjustments.onValueChanged.AddListener(ToggleColorAjustments);
    }

    public void ToggleBloom(bool isOn)
    {
        if (postProcessingVolume != null &&
            postProcessingVolume.profile.TryGet<Bloom>(out var bloom))
        {
            bloom.active = isOn;
        }
    }

    public void TogglePostProcessing(bool isOn)
    {
        if (postProcessingVolume != null)
        {
            postProcessingVolume.enabled = isOn;
        }
    }

    public void ToggleTonemapping(bool isOn)
    {
        if (postProcessingVolume != null &&
            postProcessingVolume.profile.TryGet<Tonemapping>(out var tonemapping))
        {
            tonemapping.active = isOn;
        }
    }

    public void ToggleColorAjustments(bool isOn)
    {
        if (postProcessingVolume != null &&
            postProcessingVolume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            colorAdjustments.active = isOn;
        }
    }

    public override void Show()
    {
        base.Show();
        FindObjectOfType<Hud>(includeInactive: true).gameObject.SetActive(false);
    }

    public override void Hide()
    {
        base.Hide();
        FindObjectOfType<Hud>(includeInactive: true).gameObject.SetActive(true);
    }
}
