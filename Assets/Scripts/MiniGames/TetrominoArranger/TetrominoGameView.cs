using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TetrominoGameView : Popup
{
    [SerializeField] private TetrominoGrid tetrominoGrid;
    [SerializeField] private TetrominoDragHandler tetrominoDragHandler;
    [SerializeField] private Button resetButton, closeButton;
    [SerializeField] private Color tetrominoColor = Color.white;
    [Range(0.1f, 5f)]
    [SerializeField] private float endGameDurationSeconds = 1f;
    private UniTaskCompletionSource endGameTcs = new();
    [SerializeField] private GameEvents gameEvents;

    //public UnityEvent onTetrominoMoved { get; set; } = new();
    //public UnityEvent onReset { get; set; } = new();

    private void Start()
    {
        tetrominoDragHandler.OnTetrominoMoved.AddListener(gameEvents.OnMoveMade.Invoke);
        tetrominoDragHandler.OnReset.AddListener(gameEvents.OnReset.Invoke);

        tetrominoGrid.onEndGame.AddListener(EndGame);
        closeButton.onClick.AddListener(Hide);
        resetButton.onClick.AddListener(tetrominoDragHandler.ResetGame);

        ChangeTetrominosColor(tetrominoColor);
    }

    private void ChangeTetrominosColor(Color color)
    {
        foreach (var tetromino in tetrominoDragHandler.GetTetrominos())
        {
            tetromino.SetColor(color);
        }
    }

    [ContextMenu("Change Tetrominos Color")]
    private void ChangeTetrominosColor() => ChangeTetrominosColor(tetrominoColor);

    [ContextMenu("Start Game")]
    public void StartGame() => StartGameAsync().Forget();
    public async UniTask StartGameAsync()
    {
        Show();

        gameEvents.OnGameStarted?.Invoke();

        Hud.Instance.HideHud();

        await endGameTcs.Task;
    }

    [ContextMenu("Finish Game")]
    public void EndGame() => EndGameAsync().Forget();

    private async UniTask EndGameAsync()
    {
        await UniTask.Delay((int)(endGameDurationSeconds * 1000));

        Hide();
        Hud.Instance.ShowHud();

        gameEvents.OnGameFinished?.Invoke();

        endGameTcs?.TrySetResult();
    }
}

