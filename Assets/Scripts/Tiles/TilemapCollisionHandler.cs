using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapCollisionHandler : MonoBehaviour
{
    private GameObject playerRoot;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tilemap collisionTilemap;
    [SerializeField] private TileCollisionData tileCollisionData;
    [SerializeField] private Tile collisionTile;
    private Vector3Int previousPosition = default;

    private void Start()
    {
        playerRoot = GameObject.FindWithTag("Player").transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        Vector3Int playerGridPosition = tilemap.WorldToCell(playerRoot.transform.position);
        SetCollisionTiles(playerGridPosition);
    }

    private void SetCollisionTiles(Vector3Int position)
    {
        if (GetLeftNeighbour(position) != null)
        {
            if (tileCollisionData.HasRightCollision(GetLeftNeighbour(position)))
            {
                collisionTilemap.SetTile(position + Vector3Int.left, collisionTile);
            }
        }
        if (GetUpNeighbour(position) != null)
        {
            if (tileCollisionData.HasBottomCollision(GetUpNeighbour(position)))
            {
                collisionTilemap.SetTile(position + Vector3Int.up, collisionTile);
            }
        }
        if (GetRightNeighbour(position) != null)
        {
            if (tileCollisionData.HasLeftCollision(GetRightNeighbour(position)))
            {
                collisionTilemap.SetTile(position + Vector3Int.right, collisionTile);
            }
        }
        if (GetDownNeighbour(position) != null)
        {
            if (tileCollisionData.HasTopCollision(GetDownNeighbour(position)))
            {
                collisionTilemap.SetTile(position + Vector3Int.down, collisionTile);
            }
        }
        //if (previousPosition == position) return;

        //previousPosition = position;
        collisionTilemap.ClearAllTiles();
    }

    private Tile GetLeftNeighbour(Vector3Int position)
    {
        return tilemap.GetTile<Tile>(position + Vector3Int.left);
    }
    private Tile GetUpNeighbour(Vector3Int position)
    {
        return tilemap.GetTile<Tile>(position + Vector3Int.up);
    }
    private Tile GetRightNeighbour(Vector3Int position)
    {
        return tilemap.GetTile<Tile>(position + Vector3Int.right);
    }
    private Tile GetDownNeighbour(Vector3Int position)
    {
        return tilemap.GetTile<Tile>(position + Vector3Int.down);
    }

    private IEnumerable<Vector3Int> GetNeighboursPositions(Vector3Int position)
    {
        yield return position + Vector3Int.left;
        yield return position + Vector3Int.up;
        yield return position + Vector3Int.right;
        yield return position + Vector3Int.down;
    }
}
