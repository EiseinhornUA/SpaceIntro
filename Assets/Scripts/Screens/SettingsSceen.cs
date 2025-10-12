using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreen : MonoBehaviour
{
    private SoundVariables soundVariables;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private VolumeSourcePair[] volumeSourcePairs;

    private void Awake()
    {
        SaveDefaultVolumes();

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);

        soundVariables = FindObjectOfType<SoundVariables>();
        //if (!soundVariables) return;
        //musicSlider.value = soundVariables.musicSliderValue;
        //SFXSlider.value = soundVariables.SFXSliderValue;
    }

    private void SetMusicVolume(float value)
    {
        audioManager.musicSource.volume = value;
    }

    private void SetSFXVolume(float value)
    {
        foreach (var volumeSourcePair in volumeSourcePairs)
            volumeSourcePair.audioSource.volume = value * volumeSourcePair.defaultVolume;
    }

    private void SaveDefaultVolumes()
    {
        foreach (var volumeSourcePair in volumeSourcePairs)
        {
            volumeSourcePair.defaultVolume = volumeSourcePair.audioSource.volume;
        }
    }
}

[Serializable]
public class VolumeSourcePair
{
    public AudioSource audioSource;
    public float defaultVolume { get; set; }
}
