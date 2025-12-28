using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class RadioScreen : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioSource radioSource;
    private List<AudioClip> radioPlaylist;
    private int playlistIndex = 0;
    private bool radioTurnedOn;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button setDefaultValuesButton;
    private float musicSliderValue = 0;

    private void Awake()
    {
        radioPlaylist = audioManager.radioPlaylist;
        radioSource.clip = radioPlaylist[0];
        volumeSlider.onValueChanged.AddListener(SetRadioVolume);
    }
    private void SetRadioVolume(float value)
    {
        radioSource.volume = value;
    }

    void Update()
    {
        if (radioTurnedOn && radioSource.timeSamples >= radioSource.clip.samples - 1)
        {
            AutoPlayNext();
        }
    }

    public void Play()
    {
        if (radioTurnedOn == false)
        {
            radioSource.Play();
            musicSliderValue = musicSlider.value;
            musicSlider.enabled = false;
            setDefaultValuesButton.enabled = false;
            musicSlider.value = 0;
        }
        radioTurnedOn = true;
    }

    public void Stop()
    {
        if (radioTurnedOn == true)
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
