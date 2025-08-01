using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "CustomTile", menuName = "ScriptableObjects/CustomTile", order = 1)]
public class CustomizableTile : Tile
{
    [SerializeField]
    private float speed;
}
