using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName ="TilemapData/LevelData")]
    public class LevelScriptable : ScriptableObject
    {
    public List<CostumeTileData> tilesToGenerate;
    }

[System.Serializable]
public struct CostumeTileData
{
    public Vector3Int _tilePos;
    public TileBase _tile;
    public CostumeTileData(Vector3Int _tilePos, TileBase tile)
    {
        this._tilePos = _tilePos;
        _tile = tile;
    }
    }
