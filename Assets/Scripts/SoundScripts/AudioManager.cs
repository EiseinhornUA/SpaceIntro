using System;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField] private List<AudioSource> soundsToPlayOnFirstFloor = new List<AudioSource>();
    [SerializeField] private List<AudioSource> soundsToPlayOnSecondFloor = new List<AudioSource>();
    [SerializeField] private List<AudioSource> soundsToPlayOnThirdFloor = new List<AudioSource>();

    private enum FloorState
    {
        None,
        OnFirstFloor,
        OnSecondFloor,
        OnThirdFloor
    }

    [SerializeField] private FloorState floorState = FloorState.None;
    private FloorState previousFloorState = FloorState.None;
    private float positionBetween1stAnd2ndFloor = 3.5f;
    private float positionBetween2ndAnd3rdFloor = 8.5f;

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

        previousFloorState = floorState;
        floorState = GetCurrentFloor();

        if (floorState != previousFloorState)
        {
            OnFloorChanged(floorState);
        }

        playerVelocity = (player.transform.position - lastPosition) / Time.deltaTime;
        lastPosition = player.transform.position;
    }

    private void OnFloorChanged(FloorState floorState)
    {
        var soundsToPlay = GetSoundsToPlay(floorState);
        foreach (var sound in soundsToPlay)
        {
            sound.loop = true;
            sound.Play();
        }
        var previousSoundsToStop = GetSoundsToPlay(previousFloorState);
        foreach (var sound in previousSoundsToStop)
        {
            sound.loop = false;
            sound.Stop();
        }
    }

    private IEnumerable<AudioSource> GetSoundsToPlay(FloorState floorState) => floorState switch
    {
        FloorState.OnFirstFloor => soundsToPlayOnFirstFloor,
        FloorState.OnSecondFloor => soundsToPlayOnSecondFloor,
        FloorState.OnThirdFloor => soundsToPlayOnThirdFloor,
        _ => Enumerable.Empty<AudioSource>(),
    };

    private FloorState GetCurrentFloor()
    {
        if (player.transform.position.y < positionBetween1stAnd2ndFloor)
        {
            return FloorState.OnFirstFloor;       
        }

        if (player.transform.position.y < positionBetween2ndAnd3rdFloor)
        {
            return FloorState.OnSecondFloor;
        }

        return FloorState.OnThirdFloor;
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
