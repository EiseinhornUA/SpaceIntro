using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource SFXSource;

    [Header("---Background---")]
    [SerializeField] public List<AudioClip> backgroundPlaylist;

    [Header("---Player---")]
    [SerializeField] public List<AudioClip> stepSounds;

    [Header("---Objects---")]
    [SerializeField] private AudioSource pipeSteamTop;
    [SerializeField] private AudioSource pipeSteamBottom;
    
    private int musicIndex = 0;

    [SerializeField] public Player player;
    public Vector3 playerVelocity;
    private Vector3 lastPosition;


    private void Awake()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (!musicSource.isPlaying && backgroundPlaylist.Count > 0)
        {
            PlayNext();
        }

        const int positionBetweenFloors = 8;
        if (player.transform.position.y < positionBetweenFloors)
        {
            if (!pipeSteamBottom.loop)
            {
                pipeSteamTop.Stop();
                pipeSteamTop.loop = false;
                pipeSteamBottom.Play();
                pipeSteamBottom.loop = true;
            }
        }
        else
        {
            if (!pipeSteamTop.loop)
            {
                pipeSteamBottom.Stop();
                pipeSteamBottom.loop = false;
                pipeSteamTop.Play();
                pipeSteamTop.loop = true;
            }
        }

        playerVelocity = (player.transform.position - lastPosition) / Time.deltaTime;
        lastPosition = player.transform.position;
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
