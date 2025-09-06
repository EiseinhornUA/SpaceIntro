using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TetrominoDragHandler : MonoBehaviour
{
    private List<Tetromino> tetrominos = new();
    [SerializeField] private TetrominoGrid tetrominoGrid;
    [SerializeField] private Button rotateTetrominoButton;
    private Tetromino selectedTetromino;

    private void Start()
    {
        tetrominos = GetTetrominos().ToList();
        SubscribeToTetrominos();

        rotateTetrominoButton.onClick.AddListener(RotateTetromino);
    }

    private void RotateTetromino()
    {
        if (selectedTetromino)
        {
            selectedTetromino.RotateClockwise();
            tetrominoGrid.FreeCellsFrom(selectedTetromino);
            OnEndDrag(selectedTetromino);
        }
    }

    private void SubscribeToTetrominos()
    {
        foreach (Tetromino tetromino in tetrominos)
        {
            tetromino.onBeginDrag.AddListener(() => OnBeginDrag(tetromino));
            tetromino.onDrag.AddListener(() => OnDrag(tetromino));
            tetromino.onEndDrag.AddListener(() => OnEndDrag(tetromino));
        }
    }

    private void OnBeginDrag(Tetromino tetromino)
    {
        MoveToFront(tetromino);
        selectedTetromino = tetromino;
        tetrominoGrid.FreeCellsFrom(tetromino);
    }

    private void OnDrag(Tetromino tetromino)
    {
        SetToPointerPosition(tetromino);
    }

    private void OnEndDrag(Tetromino tetromino)
    {
        Vector2Int position = tetrominoGrid.GetGridPositionFrom(tetromino.GetPlacementPosition());

        if (IsPossibleToPlaceAt(tetromino, position))
        {
            PlaceToGrid(tetromino, position);
        }
    }

    private bool IsPossibleToPlaceAt(Tetromino tetromino, Vector2Int position)
    {
        foreach(var tetrominoPosition in tetromino.GetGridCellPositions(position))
        {
            if (tetrominoGrid.IsCellOcupied(tetrominoPosition)) return false;
            if (tetrominoGrid.IsPositionOutOfGrid(tetrominoPosition)) return false;
        }
        return true;
    }

    private void PlaceToGrid(Tetromino tetromino, Vector2Int gridPosition)
    {
        SetToGridAlignedPosition(tetromino, gridPosition);

        List<Vector2Int> positions = tetromino.GetGridCellPositions(gridPosition).ToList();

        tetrominoGrid.OccupyCells(tetromino, positions);
    }

    //private bool IsOccupied(Vector2Int position)
    //{
    //    return !occupiedCells.Any(tp => tp.positions.Contains(position));
    //}

    private void MoveToFront(Tetromino tetromino)
    {
        tetromino.transform.SetAsLastSibling();
    }

    private void SetToPointerPosition(Tetromino tetromino)
    {
        tetromino.transform.position = Pointer.current.position.ReadValue();
    }
    
    private void SetToGridAlignedPosition(Tetromino tetromino, Vector2Int gridPosition)
    {
        tetromino.transform.localPosition = tetrominoGrid.GetGridAlignedPositionFrom(gridPosition) - tetromino.GetPlacementRelativePosition();
    }

    private IEnumerable<Tetromino> GetTetrominos()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<Tetromino>(out var tetromino))
            {
                yield return tetromino;
            }
        }
    }
}

