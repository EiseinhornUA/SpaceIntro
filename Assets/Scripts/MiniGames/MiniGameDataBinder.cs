using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MiniGameDataBinder : MonoBehaviour
{
    [SerializeField] private GameEvents gameActions;
    [SerializeField] private MiniGameDataSaver dataSaver;
    [SerializeField] private string gameName;

    private void Awake()
    {
        gameActions.OnGameStarted.AddListener(dataSaver.ActivateTimer);
        gameActions.OnMoveMade.AddListener(dataSaver.AddMove);
        gameActions.OnReset.AddListener(dataSaver.TryToAddResetScore);
        gameActions.OnGameFinished.AddListener(OnGameFinished);
    }

    private void OnGameFinished()
    {
        dataSaver.DeactivateTimer();
        dataSaver.SaveMiniGame(gameName);
        gameActions.OnGameStarted.RemoveAllListeners();
        gameActions.OnMoveMade.RemoveAllListeners();
        gameActions.OnReset.RemoveAllListeners();
        gameActions.OnGameFinished.RemoveAllListeners();
    }
}
