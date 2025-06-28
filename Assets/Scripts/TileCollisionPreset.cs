using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "ScriptableObjects/Collision Preset")]
public class CollisionPreset : ScriptableObject
{
    public CollisionFlags flags;
    public List<Tile> tiles = new List<Tile>();
}
