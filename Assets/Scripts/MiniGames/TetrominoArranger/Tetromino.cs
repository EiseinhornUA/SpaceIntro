using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Tetromino : MonoBehaviour
{
    [SerializeField] private TetrominoSO tetrominoSO;
    [SerializeField] private RectTransform placementPoint;
    private List<Vector2Int> cellPositions;
    [SerializeField] private TetrominoView tetrominoView;
    public UnityEvent onBeginDrag => tetrominoView.onBeginDrag;
    public UnityEvent onDrag => tetrominoView.onDrag;
    public UnityEvent onEndDrag => tetrominoView.onEndDrag;
    public UnityEvent onClick => tetrominoView.onClick;

    private void Start()
    {
        cellPositions = new List<Vector2Int>(tetrominoSO.cellPositions);
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

    public void RotateClockwise()
    {
        cellPositions = cellPositions.ConvertAll(pos => new Vector2Int(pos.y, -pos.x));

        placementPoint.localPosition = new Vector2(placementPoint.localPosition.y, -placementPoint.localPosition.x);

        tetrominoView.RotateClockwise();
    }
}