using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideFromPlayer : MonoBehaviour
{
    private TileBase[] treesOnScene;
    private Tilemap tilemap;
    private SortingLayer trees;
    private PlayerController player;


    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        BoundsInt bounds = tilemap.cellBounds;
       // treesOnScene = GetTreeTiles;

        
    }
    private void HideTilesFromPlayer()
    {
        BoundsInt bounds = tilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)

            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(tilePos);
                if (tile != null && tilePos.y > player.transform.position.y)
                {
                    tilemap.GetComponent<SpriteRenderer>().sortingLayerName = trees.name;
                }
                else
                {
                    Debug.Log("x:" + x + " y:" + y + " tile: (null)");
                }

            }
    }
}