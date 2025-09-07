using DG.Tweening;
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
    [SerializeField] private float rotationDurationSeconds = 0.125f;
    [SerializeField] private float placementDurationSeconds = 0.125f;
    private Tetromino selectedTetromino;
    private bool isDragingTetromino;

    private void Start()
    {
        tetrominos = GetTetrominos().ToList();
        SubscribeToTetrominos();

        rotateTetrominoButton.onClick.AddListener(RotateTetromino);
    }

    private void RotateTetromino()
    {
        if (!selectedTetromino) return;

        selectedTetromino.RotateClockwise(rotationDurationSeconds);
        tetrominoGrid.FreeCellsFrom(selectedTetromino);
        OnEndDrag(selectedTetromino);
    }

    private void SubscribeToTetrominos()
    {
        foreach (Tetromino tetromino in tetrominos)
        {
            tetromino.onBeginDrag.AddListener(() => OnBeginDrag(tetromino));
            tetromino.onDrag.AddListener(() => OnDrag(tetromino));
            tetromino.onEndDrag.AddListener(() => OnEndDrag(tetromino));
            tetromino.onClick.AddListener(() => OnClick(tetromino));
        }
    }

    private void OnClick(Tetromino tetromino)
    {
        if (isDragingTetromino) return;

        MoveToFront(tetromino);

        if (selectedTetromino)
            selectedTetromino.HideOutline();
        
        tetromino.ResetOutlinePoistion();
        tetromino.ShowOutline();
        tetromino.RenderOutlineAbove();

        if (tetromino == selectedTetromino)
            RotateTetromino();

        selectedTetromino = tetromino;
    }


    private void OnBeginDrag(Tetromino tetromino)
    {
        isDragingTetromino = true;

        if (selectedTetromino)
            selectedTetromino.HideOutline();

        tetromino.HideOutline();

        MoveToFront(tetromino);
        tetrominoGrid.FreeCellsFrom(tetromino);

        selectedTetromino = tetromino;
    }

    private void OnDrag(Tetromino tetromino)
    {
        SetToPointerPosition(tetromino);

        Vector2Int position = tetrominoGrid.GetGridPositionFrom(tetromino.GetPlacementPosition());

        if (IsPossibleToPlaceAt(tetromino, position))
        {
            if (!tetromino.IsOutlineVisible())
            {
                tetromino.ShowOutline();
                tetromino.RenderOutlineBelow();
            }

            PlaceOutlineToGrid(tetromino, position);
            return;
        }
        if (tetromino.IsOutlineVisible())
        {
            tetromino.ResetOutlinePoistion();
            tetromino.HideOutline();
        }
    }

    private void OnEndDrag(Tetromino tetromino)
    {
        Vector2Int position = tetrominoGrid.GetGridPositionFrom(tetromino.GetPlacementPosition());

        if (IsPossibleToPlaceAt(tetromino, position))
        {
            PlaceToGrid(tetromino, position);
            MoveToBack(tetromino);
        }

        tetromino.ResetOutlinePoistion();
        tetromino.ShowOutline();
        tetromino.RenderOutlineAbove();

        isDragingTetromino = false;
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

    private void PlaceOutlineToGrid(Tetromino tetromino, Vector2Int gridPosition)
    {
        SetOutlineToGridAlignedPosition(tetromino, gridPosition);
    }

    //private bool IsOccupied(Vector2Int position)
    //{
    //    return !occupiedCells.Any(tp => tp.positions.Contains(position));
    //}

    private void MoveToFront(Tetromino tetromino)
    {
        tetromino.transform.SetAsLastSibling();
    }

    private void MoveToBack(Tetromino tetromino)
    {
        tetromino.transform.SetAsFirstSibling();
    }

    private void SetToPointerPosition(Tetromino tetromino)
    {
        tetromino.transform.position = Pointer.current.position.ReadValue();
    }
    
    private void SetToGridAlignedPosition(Tetromino tetromino, Vector2Int gridPosition)
    {
        tetromino.transform.DOLocalMove(tetrominoGrid.GetGridAlignedPositionFrom(gridPosition) - tetromino.GetPlacementRelativePosition(), placementDurationSeconds);
    }

    private void SetOutlineToGridAlignedPosition(Tetromino tetromino, Vector2Int gridPosition)
    {
        tetromino.SetOutlinePosition(tetrominoGrid.GetGridAlignedPositionFrom(gridPosition) - tetromino.GetPlacementRelativePosition());
    }

    public IEnumerable<Tetromino> GetTetrominos()
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

