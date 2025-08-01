using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Flags]
public enum CollisionFlags
{
    None = 0,
    Top = 1 << 0,
    Bottom = 1 << 1,
    Left = 1 << 2,
    Right = 1 << 3
}

[CreateAssetMenu(fileName = "TileCollisionData", menuName = "ScriptableObjects/TileCollisionData", order = 1)]
public class TileCollisionData : ScriptableObject
{
    public List<CollisionPreset> presets = new List<CollisionPreset>();

    private Dictionary<Tile, CollisionFlags> collisionMap;

    private void OnValidate()
    {
        RebuildMap();
    }

    public void RebuildMap()
    {
        collisionMap = presets
            .SelectMany(preset => preset.tiles.Select(tile => (tile, preset.flags)))
            .ToDictionary(x => x.tile, x => x.flags);
    }

    public CollisionFlags GetCollisions(Tile tile)
    {
        if (collisionMap == null || collisionMap.Count == 0)
            RebuildMap();

        return collisionMap.TryGetValue(tile, out var flags) ? flags : CollisionFlags.None;
    }

    public bool HasTopCollision(Tile tile)
    {
        return (GetCollisions(tile) & CollisionFlags.Top) != 0;
    }

    public bool HasBottomCollision(Tile tile)
    {
        return (GetCollisions(tile) & CollisionFlags.Bottom) != 0;
    }

    public bool HasLeftCollision(Tile tile)
    {
        return (GetCollisions(tile) & CollisionFlags.Left) != 0;
    }

    public bool HasRightCollision(Tile tile)
    {
        return (GetCollisions(tile) & CollisionFlags.Right) != 0;
    }
}
