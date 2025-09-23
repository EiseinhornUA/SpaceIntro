using System;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    private AudioManager audioManager;
    public AudioSource SFXSource;
    private AudioSource musicSource;
    [SerializeField] public AudioClip clipToPlay;

    private Vector3 lastPosition;
    private Vector3 velocity;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        SFXSource = audioManager.SFXSource;
        musicSource = audioManager.musicSource;

        lastPosition = transform.position;
    }

    public void PlaySound(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    private void Update()
    {
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }

    //Called from animator
    public void Step()
    {
        if (Math.Abs(velocity.x) > 0.1f)
        {
            PlaySound(audioManager.stepSounds[UnityEngine.Random.Range(0, audioManager.stepSounds.Count)]);
        }
    }

    public void Play() => PlaySound(clipToPlay);
}
