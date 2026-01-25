using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class RadioScreen : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioSource radioSource;
    [SerializeField] private TextMeshProUGUI radioClipName;
    private List<AudioClip> radioPlaylist;
    private int playlistIndex = 0;
    private bool radioTurnedOn;
    private AudioClip previousRadioClip;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button setDefaultValuesButton;
    private float musicSliderValue = 0;

    [SerializeField] private GameObject radioButton;
    private bool isRadioScreenShown = false;
    public bool IsRadioScreenShown() => isRadioScreenShown;

    public void SetRadioScreenBoolTrue()
    {
        isRadioScreenShown = true;
    }
    public void ShowRadioButton()
    {
        SetRadioScreenBoolTrue();
        radioButton.SetActive(true);
        radioButton.transform.GetChild(1).gameObject.SetActive(false);
    }

    private void Awake()
    {
        radioPlaylist = audioManager.radioPlaylist;
        radioSource.clip = radioPlaylist[0];
        radioClipName.text = $"{radioSource.clip.name}";
        previousRadioClip = radioPlaylist[0];
        volumeSlider.onValueChanged.AddListener(SetRadioVolume);
    }
    private void SetRadioVolume(float value)
    {
        radioSource.volume = value;
    }

    void Update()
    {
        if (isMusicStopped())
        {
            AutoPlayNext();
        }
        if (radioSource.clip.name != previousRadioClip.name)
        {
            radioClipName.text = $"{radioSource.clip.name}";

            previousRadioClip = radioSource.clip;
        }
    }

    private bool isMusicStopped()
    {
        return radioTurnedOn && radioSource.timeSamples >= radioSource.clip.samples - 1;
    }

    public void Play()
    {
        if (!radioTurnedOn)
        {
            radioSource.Play();
            musicSliderValue = musicSlider.value;
            musicSlider.enabled = false;
            Debug.Log(">>> Disabling music slider");
            setDefaultValuesButton.enabled = false;
            musicSlider.value = 0;
        }
        radioTurnedOn = true;
    }

    public void Stop()
    {
        if (radioTurnedOn)
        {
            musicSlider.value = musicSliderValue;
            musicSlider.enabled = true;
            setDefaultValuesButton.enabled = true;
            radioSource.Pause();
        }
        radioTurnedOn = false;
    }

    public void Next()
    {
        bool radioSourcePlayed = radioSource.isPlaying;
        radioSource.Stop();
        radioSource.clip = GetNextClip();
        if (radioSourcePlayed)
        {
            radioSource.Play();
        }
    }

    public void Previous()
    {
        bool radioSourcePlayed = radioSource.isPlaying;
        radioSource.Stop();
        radioSource.clip = GetPreviousClip();
        if (radioSourcePlayed)
        {
            radioSource.Play();
        }
    }

    private void AutoPlayNext()
    {
        radioSource.clip = GetNextClip();
        radioSource.Play();
    }

    private AudioClip GetNextClip()
    {
        playlistIndex++;
        if (playlistIndex >= radioPlaylist.Count)
        {
            playlistIndex -= radioPlaylist.Count;
        }
        return radioPlaylist[playlistIndex];
    }

    private AudioClip GetPreviousClip()
    {
        playlistIndex--;
        if (playlistIndex < 0)
        {
            playlistIndex += radioPlaylist.Count;
        }
        return radioPlaylist[playlistIndex];
    }
}
