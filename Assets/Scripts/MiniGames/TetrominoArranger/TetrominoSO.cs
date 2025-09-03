using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TetrominoSO", menuName = "ScriptableObjects/TetrominoSO", order = 1)]
public class TetrominoSO : ScriptableObject
{
    [field: SerializeField] public List<Vector2Int> positions { get; private set; }
}