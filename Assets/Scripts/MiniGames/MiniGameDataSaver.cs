using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameDataSaver : MonoBehaviour
{
    public int numberOfTries = 0;
    [SerializeField] private int resetCooldown = 7;
    public float resetTimer = 0f;
    public int amountOfMoves = 0;

    private float floatTimeUsedToFinish = 0;
    public int timeUsedToFinish = 0;
    public bool isTimerActive = false;
    public bool isReadingPopup = false;
    [SerializeField] private int timeToShowSkipPopup = 120;

    [SerializeField] private GameObject skipPopup;

    [SerializeField] private DatabaseManager databaseManager;
    

    private void Update()
    {
        if (resetTimer > 0f)
        {
            resetTimer -= Time.deltaTime;
            if (resetTimer < 0f)
                resetTimer = 0f;
        }

        if (isTimerActive)
        {
            if (!isReadingPopup)
            {
                floatTimeUsedToFinish += Time.deltaTime;
                timeUsedToFinish = (int)floatTimeUsedToFinish;
            }
        }

        if ((floatTimeUsedToFinish % timeToShowSkipPopup) < Time.deltaTime && timeUsedToFinish > 0)
        {
            isReadingPopup = true;
            skipPopup.SetActive(true);
        }
    }

    public void TryToAddResetScore()
    {
        if (resetTimer == 0)
            numberOfTries += 1;
        resetTimer = resetCooldown;
    }

    public void ActivateTimer()
    {
        isTimerActive = true;
    }

    public void DeactivateTimer()
    {
        isTimerActive = false;
    }

    public void AddMove()
    {
        amountOfMoves += 1;
    }

    public void HideSkipPopUp()
    {
        isReadingPopup = false;
        skipPopup.SetActive(false);
    }

    public void SaveMiniGame(string gameName)
    {
        databaseManager.SaveMiniGameTime(timeUsedToFinish, gameName);
        databaseManager.SaveMiniGameAttempts(numberOfTries, gameName);
        databaseManager.SaveMiniGameMoves(amountOfMoves, gameName);
    }

    //public void SaveTetrominoGame() => SaveMiniGame("Tetromino");
    //public void SaveBoltCryoTerminalGame() => SaveMiniGame("BoltGameCryoTerminal");
    //public void SaveBoltRobotGame() => SaveMiniGame("BoltGameRobot");
}
