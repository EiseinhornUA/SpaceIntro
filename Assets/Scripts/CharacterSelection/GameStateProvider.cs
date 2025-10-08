using System;

public static class GameStateProvider
{
    public static GameState State { get; private set; } = GameState.MainMenu;

    public enum GameState : int
    {
        MainMenu,
        Started,
        Completed,
        Continued,
    }

    public static void SetCompleted() => State = GameState.Completed;
    public static void SetStarted() => State = GameState.Started;
    public static void SetContinued() => State = GameState.Continued;

    public static bool IsGameCompleted()
    {
        return State == GameState.Completed;
    }

    public static bool IsGameContinued()
    {
        return State == GameState.Continued;
    }

    public static bool IsGameStarted()
    {
        return State == GameState.Started;
    }

    internal static bool IsMainMenu()
    {
        return State == GameState.MainMenu;
    }

    internal static void SetState(GameState gameState)
    {
        State = gameState;
    }
}