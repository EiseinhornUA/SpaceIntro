using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Tetromino : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TetrominoSO tetrominoSO;
    public UnityEvent onBeginDrag { get; set; } = new();
    public UnityEvent onDrag { get; set; } = new();
    public UnityEvent onEndDrag { get; set; } = new();

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag.Invoke();
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDrag.Invoke();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onEndDrag.Invoke();
    }

    public IEnumerable<Vector2Int> GetGridPositionsByPosition(Vector2Int position)
    {
        foreach (var tetrominoPosition in tetrominoSO.positions)
        {
            yield return new Vector2Int(tetrominoPosition.x + position.x, tetrominoPosition.y + position.y);
        }
    }
}