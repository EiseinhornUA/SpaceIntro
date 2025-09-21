using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource SFXSource;

    [Header("---Background---")]
    public List<AudioClip> backgroundPlaylist;

    [Header("---Player---")]
    public List<AudioClip> stepSounds;
    public AudioClip pickUpItem;

    [Header("---Robot---")]
    public AudioClip robotIdle;

    [Header("---Objects---")]
    public AudioClip cryoChamberOpened;
    public AudioClip doorOpening;

    [Header("---UI---")]
    public AudioClip pdaOpen;
    public AudioClip pdaClose;
    private int musicIndex = 0;

    private void Start()
    {
        //if (backgroundPlaylist.Count > 0)
        //{
        //    PlayNext();
        //}
    }

    void Update()
    {
        if (!musicSource.isPlaying && backgroundPlaylist.Count > 0)
        {
            PlayNext();
        }
    }

    private void PlayNext()
    {
        musicSource.clip = backgroundPlaylist[musicIndex];
        musicSource.Play();

        musicIndex++;

        if (musicIndex >= backgroundPlaylist.Count)
        {
            musicIndex = 0;
        }
    }
}
