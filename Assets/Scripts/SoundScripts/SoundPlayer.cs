using System;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    private AudioManager audioManager;
    public AudioSource SFXSource;
    private AudioSource musicSource;
    [SerializeField] public AudioClip clipToPlay;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        SFXSource = audioManager.SFXSource;
        musicSource = audioManager.musicSource;
    }

    public void PlaySound(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    //Called from animator
    public void Step()
    {
        if (Math.Abs(audioManager.playerVelocity.x) > 0.1f)
        {
            PlaySound(audioManager.stepSounds[UnityEngine.Random.Range(0, audioManager.stepSounds.Count)]);
        }
    }

    public void Play() => PlaySound(clipToPlay);
}
