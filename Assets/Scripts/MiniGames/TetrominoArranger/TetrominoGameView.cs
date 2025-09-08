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

    private void Start()
    {
        tetrominoGrid.onEndGame.AddListener(() => UniTask.Void(async () => await OnEndGame()));
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

    private async UniTask OnEndGame()
    {
        await UniTask.Delay((int)(endGameDurationSeconds * 1000));
        Hide();
    }
}

