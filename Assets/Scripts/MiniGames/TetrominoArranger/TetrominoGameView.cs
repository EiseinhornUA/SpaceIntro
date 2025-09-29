using Cysharp.Threading.Tasks;
using UnityEngine;
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

    private void Start()
    {
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

    public async UniTask StartGameAsync()
    {
        Show();

        Hud.Instance.HideHud();

        await endGameTcs.Task;
    }

    private void EndGame() => EndGameAsync().Forget();

    private async UniTask EndGameAsync()
    {
        await UniTask.Delay((int)(endGameDurationSeconds * 1000));

        Hide();
        Hud.Instance.ShowHud();

        endGameTcs?.TrySetResult();
    }
}

