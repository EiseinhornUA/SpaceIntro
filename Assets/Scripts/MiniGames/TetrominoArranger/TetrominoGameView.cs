using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TetrominoGameView : Popup
{
    [SerializeField] private TetrominoGrid tetrominoGrid;
    [SerializeField] private TetrominoDragHandler tetrominoDragHandler;
    [SerializeField] private Button closeButton;
    [SerializeField] private Color tetrominoColor = Color.white;
    [Range(0.1f, 5f)]
    [SerializeField] private float endGameDurationSeconds = 1f;

    private void Start()
    {
        tetrominoGrid.onEndGame.AddListener(() => UniTask.Void(async () => await OnEndGame()));
        closeButton.onClick.AddListener(Hide);

        foreach (var tetromino in tetrominoDragHandler.GetTetrominos())
        {
            tetromino.SetColor(tetrominoColor);    
        }
    }

    private async UniTask OnEndGame()
    {
        await UniTask.Delay((int)(endGameDurationSeconds * 1000));
        Hide();
    }

}

