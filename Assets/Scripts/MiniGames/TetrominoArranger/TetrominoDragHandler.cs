using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TetrominoDragHandler : MonoBehaviour
{
    private List<Tetromino> tetrominos = new();
    [SerializeField] private TetrominoGrid tetrominoGrid;

    private void Start()
    {
        tetrominos = GetTetrominos().ToList();
        SubscribeToTetrominos();
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
        tetrominoGrid.FreeCellsFrom(tetromino);
    }

    private void OnDrag(Tetromino tetromino)
    {
        SetToPointerPosition(tetromino);
    }

    private void OnEndDrag(Tetromino tetromino)
    {
        Vector2Int position = tetrominoGrid.GetGridPositionFrom(tetromino.GetPlacementPosition());

        Debug.Log("Trying to place at " + position);
        Debug.Log("IsPossibleToPlaceAt: " + IsPossibleToPlaceAt(tetromino, position));
        
        if (IsPossibleToPlaceAt(tetromino, position))
            PlaceToGrid(tetromino, position);
    }

    private bool IsPossibleToPlaceAt(Tetromino tetromino, Vector2Int position)
    {
        foreach(var tetrominoPosition in tetromino.GetGridPositionsByPosition(position))
        {
            if (tetrominoGrid.IsCellOcupied(tetrominoPosition)) return false;
            if (tetrominoGrid.IsPositionOutOfGrid(tetrominoPosition)) return false;
        }
        return true;
    }

    private void PlaceToGrid(Tetromino tetromino, Vector2Int gridPosition)
    {
        SetToGridAlignedPosition(tetromino, gridPosition);

        List<Vector2Int> positions = tetromino.GetGridPositionsByPosition(gridPosition).ToList();

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
        tetromino.transform.localPosition = tetrominoGrid.GetGridAlignedPositionFrom(gridPosition) - tetromino.GetLeftBottomCornerRelativePosition();
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

