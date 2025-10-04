using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tetromino : MonoBehaviour
{
    [SerializeField] private TetrominoSO tetrominoSO;
    [SerializeField] private RectTransform placementPoint;
    private List<Vector2Int> cellPositions;
    [SerializeField] private TetrominoView tetrominoView;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 initialPlacementPointPosition;

    public UnityEvent onBeginDrag => tetrominoView.onBeginDrag;
    public UnityEvent onDrag => tetrominoView.onDrag;
    public UnityEvent onEndDrag => tetrominoView.onEndDrag;
    public UnityEvent onClick => tetrominoView.onClick;

    private void Start()
    {
        cellPositions = new List<Vector2Int>(tetrominoSO.cellPositions);

        initialPosition = transform.localPosition;
        initialRotation = transform.rotation;
        initialPlacementPointPosition = placementPoint.localPosition;
    }

    public IEnumerable<Vector2Int> GetGridCellPositions(Vector2Int position)
    {
        foreach (var tetrominoPosition in cellPositions)
        {
            yield return new Vector2Int(tetrominoPosition.x + position.x, tetrominoPosition.y + position.y);
        }
    }

    public Vector2 GetPlacementPosition()
    {
        return placementPoint.position;
    }

    public Vector2 GetPlacementRelativePosition()
    {
        return placementPoint.localPosition;
    }

    public void RotateClockwise(float rotationDurationSeconds)
    {
        cellPositions = cellPositions.ConvertAll(pos => new Vector2Int(pos.y, -pos.x));

        placementPoint.localPosition = new Vector2(placementPoint.localPosition.y, -placementPoint.localPosition.x);

        tetrominoView.RotateClockwise(rotationDurationSeconds);
    }

    public void ShowOutline() => tetrominoView.ShowOutline();
    public void HideOutline() => tetrominoView.HideOutline();
    public void SetOutlinePosition(Vector2 position) => tetrominoView.SetOutlinePosition(position, transform.localPosition);
    public bool IsOutlineVisible() => tetrominoView.IsOutlineVisible();
    public void ResetOutlinePoistion() => tetrominoView.ResetOutlinePosition();
    public void RenderOutlineAbove() => tetrominoView.RenderOutlineAbove();
    public void RenderOutlineBelow() => tetrominoView.RenderOutlineBelow();
    public void SetColor(Color tetrominoColor) => tetrominoView.SetColor(tetrominoColor);

    public void ResetRotation(float durationSeconds)
    {
        tetrominoView.ResetRotation(durationSeconds);
        placementPoint.localPosition = initialPlacementPointPosition;
        cellPositions = new List<Vector2Int>(tetrominoSO.cellPositions);
    }

    public void ResetPosition(float durationSeconds)
    {
        transform.DOLocalMove(initialPosition, durationSeconds);
    }
}