using UnityEngine;
using UnityEngine.UI;

public class SoundVariables : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    public float musicSliderValue;
    public float SFXSliderValue;

    private float defaultMusicValue;
    private float defaultSFXValue;

    private void SetMusicVolume(float value)
    {
        musicSliderValue = value;
    }

    private void SetSFXVolume(float value)
    {
        SFXSliderValue = value;
    }

    public void SetDefaultValues()
    {
        musicSlider.value = defaultMusicValue;
        SFXSlider.value = defaultSFXValue;
        musicSliderValue = defaultMusicValue;
        SFXSliderValue = defaultSFXValue;
    }

    private void Awake()
    {
        musicSliderValue = musicSlider.value;
        SFXSliderValue = SFXSlider.value;
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        SFXSlider.onValueChanged.AddListener(SetSFXVolume);
        defaultMusicValue = musicSliderValue;
        defaultSFXValue = SFXSliderValue;
        DontDestroyOnLoad(gameObject);
    }
}
