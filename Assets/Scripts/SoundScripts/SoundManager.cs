using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private AudioManager audioManager;
    private AudioSource SFXSource;
    private AudioSource musicSource;
    //[SerializeField] private AudioClip clipToPlay;

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        SFXSource = audioManager.SFXSource;
        musicSource = audioManager.musicSource;
    }

    public void PlaySound(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    private void Step() => PlaySound(audioManager.stepSounds[UnityEngine.Random.Range(0, audioManager.stepSounds.Count)]);
}
