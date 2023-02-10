using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilesInstantiator : MonoBehaviour
{
    private enum TileType { Background , Obstacles }
    [SerializeField]
    private TileType tileType;
    [SerializeField]
    private LevelScriptable levelData;
    [SerializeField]
    private Tilemap tilemap;
    [SerializeField] Vector3Int tilesPadding;
    [SerializeField]
    private NavMeshPlus.Components.NavMeshSurface nevMeshSurface;
    private void InstantiateBackground()
    {
        foreach (var obstacleData in levelData.tilesToGenerate)
        {
            tilemap.SetTile(obstacleData._tilePos * tilesPadding,obstacleData._tile);
        }
    }
    private void Awake()
    {
        switch(tileType)
        {
            case (TileType.Background):
                InstantiateBackground();
                break;
            case (TileType.Obstacles):
                nevMeshSurface.BuildNavMeshAsync();
                break;
        }
    }

}
