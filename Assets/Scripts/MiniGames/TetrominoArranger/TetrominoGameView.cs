using UnityEngine;
using UnityEngine.UI;

public class TetrominoGameView : Popup
{
    [SerializeField] private TetrominoGrid tetrominoGrid;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        tetrominoGrid.onEndGame.AddListener(OnEndGame);
        closeButton.onClick.AddListener(Hide);
    }

    private void OnEndGame()
    {
        Hide();
    }
}

