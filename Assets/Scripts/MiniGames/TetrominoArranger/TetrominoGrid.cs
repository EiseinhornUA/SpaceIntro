using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TetrominoGrid : MonoBehaviour
{
    [SerializeField] private Vector2Int gridSize;
    private List<TetrominoPositions> occupiedCells = new();

    public Vector2Int GetPointerGridPosition()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        // Convert screen pointer position to local position inside RectTransform
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            Pointer.current.position.ReadValue(),
            null,
            out Vector2 localPoint
        );

        // Normalize local point to [0,1] range
        Vector2 normalized = (localPoint + rectTransform.rect.size * 0.5f) / rectTransform.rect.size;

        // Convert normalized to grid coordinates
        return new Vector2Int(
            Mathf.FloorToInt(normalized.x * gridSize.x),
            Mathf.FloorToInt(normalized.y * gridSize.y)
        );
    }

    public Vector2 GetPointerGridAlignedPosition()
    {
        Vector2Int pointerGridPosition = GetPointerGridPosition();
        RectTransform rectTransform = GetComponent<RectTransform>();

        // Calculate cell size in local space
        Vector2 cellSize = rectTransform.sizeDelta / (Vector2)gridSize;

        // Convert grid index to local position (center of the cell)
        Vector2 alignedLocalPosition = new Vector2(
            (pointerGridPosition.x + 0.5f) * cellSize.x - rectTransform.sizeDelta.x * 0.5f,
            (pointerGridPosition.y + 0.5f) * cellSize.y - rectTransform.sizeDelta.y * 0.5f
        );

        return alignedLocalPosition;
    }

    public void OccupyCells(Tetromino tetromino, List<Vector2Int> positions)
    {
        occupiedCells.Add(new TetrominoPositions(tetromino, positions));
    }

    public bool IsCellOcupied(Vector2Int position)
    {
        foreach (var tetrominoPositions in occupiedCells)
        {
            if (tetrominoPositions.positions.Contains(position))
            {
                Debug.Log($"Cell {position} is occupied by {tetrominoPositions.tetromino.name}");
                return true;
            }
        }
        return false;
    }

    public bool IsPositionOutOfGrid(Vector2Int position)
    {
        return position.x < 0 || position.y < 0 || position.x >= gridSize.x || position.y >= gridSize.y;
    }

    public void FreeCellsFrom(Tetromino tetromino)
    {
        occupiedCells.RemoveAll(tp => tp.tetromino == tetromino);
    }
}

public class TetrominoPositions
{
    public Tetromino tetromino;
    public List<Vector2Int> positions;

    public TetrominoPositions(Tetromino tetromino, List<Vector2Int> positions)
    {
        this.tetromino = tetromino;
        this.positions = positions;
    }
}