using UnityEngine;
using UnityEngine.UI;

public class SoundVariables : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    public float musicSliderValue;
    public float SFXSliderValue;

    private void SetMusicVolume(float value)
    {
        musicSliderValue = value;
    }

    private void SetSFXVolume(float value)
    {
        SFXSliderValue = value;
    }

    private void Awake()
    {
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);
        DontDestroyOnLoad(gameObject);
    }
}
