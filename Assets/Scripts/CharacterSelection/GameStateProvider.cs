public static class GameStateProvider
{
    public static GameState State { get; private set; } = GameState.MainMenu;
    
    public enum GameState : int
    {
        MainMenu,
        Started,
        Completed,
    }

    public static void SetCompleted() => State = GameState.Completed;
    public static void SetStarted() => State = GameState.Started;

    public static bool IsGameCompleted()
    {
        return State == GameState.Completed;
    }
}