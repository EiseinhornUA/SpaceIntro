using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.Tilemaps;

public class LayerCollisionHandler : MonoBehaviour
{
    private GameObject playerRoot;
    private GameObject player;
    [SerializeField] private List<Tile> slopes;
    [Header("Debug")]
    [SerializeField] private Tile highlightTile;
    private Tilemap tilemap;
    [SerializeField] private Tilemap highlightTilemap;
    private Vector3Int highlightedCellPosition;
    private bool hasHighlightedTile;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        playerRoot = player.transform.GetChild(0).gameObject;
        tilemap = GetComponent<Tilemap>();
    }

    private void Update()
    {

        Vector3Int playerGridPosition = tilemap.WorldToCell(playerRoot.transform.position);

        if (IsOnSecondLayer())
        {
            playerGridPosition += Vector3Int.down + Vector3Int.left;
        }

        if (slopes.Contains((Tile)tilemap.GetTile(playerGridPosition)) && IsOnFirstLayer())
        {
            PlayerToSecondLayer();
            player.transform.position += Vector3.up / 2;
        }
        if (tilemap.GetTile(playerGridPosition) == null && IsOnSecondLayer())
        {
            PlayerToFirstLayer();
            player.transform.position -= Vector3.up / 2;
        }

        HighlightTile(playerGridPosition);
    }

    private bool IsOnFirstLayer()
    {
        return player.layer == LayerMask.NameToLayer("First");
    }

    private bool IsOnSecondLayer()
    {
        return player.layer == LayerMask.NameToLayer("Second");
    }

    private void PlayerToSecondLayer()
    {
        player.layer = LayerMask.NameToLayer("Second");
        player.GetComponent<SpriteRenderer>().sortingOrder = 2;
    }

    private void PlayerToFirstLayer()
    {
        player.layer = LayerMask.NameToLayer("First");
        player.GetComponent<SpriteRenderer>().sortingOrder = 1;
    }

    public void HighlightTile(Vector3Int cellPosition)
    {
        if (hasHighlightedTile && highlightedCellPosition != cellPosition)
        {
            highlightTilemap.SetTile(highlightedCellPosition, null);
            hasHighlightedTile = false;
        }

        highlightTilemap.SetTile(cellPosition, highlightTile);
        highlightedCellPosition = cellPosition;
        hasHighlightedTile = true;
    }
}
