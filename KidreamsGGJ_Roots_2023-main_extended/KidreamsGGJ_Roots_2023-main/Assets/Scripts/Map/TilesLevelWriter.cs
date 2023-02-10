using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using NavMeshPlus.Extensions;
//Create new script


public class TilesLevelWriter : MonoBehaviour
{
    [SerializeField]
    private LevelScriptable levelData;

    [SerializeField]
    private CostumeTileData[] tilesToWrite;
    [SerializeField]
    protected Vector2 backgroundSize = new Vector2(500,150);
    [SerializeField]
    private int startingX;
    [SerializeField]
    private int startingY;


/*    private void Awake()
    {
        WriteTilesData();
    }*/
    private void WriteTilesData()
    {
        levelData.tilesToGenerate.Clear();
        for (int x = startingX; x < backgroundSize.x; x++)
        {
            for (int y = startingY; y < backgroundSize.y; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y,0);
                var obstacle = new CostumeTileData(tilePos , RandomTile()._tile);
                levelData.tilesToGenerate.Add(obstacle);
            }
        }
    }

    private CostumeTileData RandomTile()
    {
        var random = UnityEngine.Random.Range(0, tilesToWrite.Length);
        return tilesToWrite[random];
    }
}
