using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class mapHandler : MonoBehaviour
{
    [Header("Map settings")]
    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private int waypointCount;
    [SerializeField] private Tilemap tilemapGround;
    [SerializeField] private Tile tileEmpty;
    [SerializeField] private Tile tileBase;
    [SerializeField] private Tile tileEnemy;
    [SerializeField] private Tile tileWaypoint;

    [Header("Path Settings")]
    [SerializeField] private Vector2Int pathStartPos;
    [SerializeField] private Vector2Int pathEndPos;
    [SerializeField][Range(0, 1)] private float windingChance; 
    
    public Vector2Int MapSize
    {
        get{return mapSize;}
    }
    private Vector2Int baseTilePos;
    private Vector2Int enemyTilePos;
    private mapQuarters baseQuarter;
    private mapEdges enemyEdge;

    [SerializeField] private List<Vector2Int> path = new List<Vector2Int>();
    
    private enum mapQuarters
    {
        DEFAULT,
        TOP_RIGHT,
        TOP_LEFT,
        BOTTOM_LEFT,
        BOTTOM_RIGHT
    }
    
    private enum mapEdges
    {
        DEFAULT,
        RIGHT,
        TOP,
        LEFT,
        BOTTOM
    }
    
    public void createMap()
    {
        for (int i = 0; i < mapSize.y; i++)
        {
            for (int j = 0; j < mapSize.x; j++)
            {
                tilemapGround.SetTile(new Vector3Int(j, i, 0), tileEmpty);
            }
        }
        
        selectBaseTile();
        baseQuarter = getMapQuarter(baseTilePos);
        selectEnemyTile();

        tilemapGround.SetTile(new Vector3Int(baseTilePos.x, baseTilePos.y, 0), tileBase);
        tilemapGround.SetTile(new Vector3Int(enemyTilePos.x, enemyTilePos.y, 0), tileEnemy);
        Debug.Log("Base quarter: " + baseQuarter);

        generatePath();
    }
    
    private mapQuarters getMapQuarter(Vector2Int tileChoords)
    {
        mapQuarters quarter = mapQuarters.DEFAULT;
        Vector2Int adjustedChoords = new Vector2Int(tileChoords.x + 1, tileChoords.y + 1);
        
        if (adjustedChoords.x > mapSize.x / 2 && adjustedChoords.y > mapSize.y / 2)
        {
            quarter = mapQuarters.TOP_RIGHT;
        }
        else if (adjustedChoords.x <= mapSize.x / 2 && adjustedChoords.y > mapSize.y / 2)
        {
            quarter = mapQuarters.TOP_LEFT;
        }
        else if (adjustedChoords.x <= mapSize.x / 2 && adjustedChoords.y <= mapSize.y / 2)
        {
            quarter = mapQuarters.BOTTOM_LEFT;
        }
        else if (adjustedChoords.x > mapSize.x / 2 && adjustedChoords.y <= mapSize.y / 2)
        {
            quarter = mapQuarters.BOTTOM_RIGHT;
        }

        return quarter;
    }
    
    private void selectBaseTile()
    {
        int xPos = UnityEngine.Random.Range(0, mapSize.x);
        int yPos = UnityEngine.Random.Range(0, mapSize.y);
        
        baseTilePos = new Vector2Int(xPos, yPos);
    }
    
    private void selectEnemyTile()
    {
        int selectSide = UnityEngine.Random.Range(0, 2);
        
        switch (baseQuarter)
        {
            case mapQuarters.TOP_RIGHT:
                if (selectSide == 1)
                {
                    enemyEdge = mapEdges.BOTTOM;
                    enemyTilePos = new Vector2Int(UnityEngine.Random.Range(0, mapSize.x / 2), 0);
                }
                else
                {
                    enemyEdge = mapEdges.LEFT;
                    enemyTilePos = new Vector2Int(0, UnityEngine.Random.Range(0, mapSize.y / 2));
                }
                
                break;
            case mapQuarters.TOP_LEFT:
                if (selectSide == 1)
                {
                    enemyEdge = mapEdges.BOTTOM;
                    enemyTilePos = new Vector2Int(UnityEngine.Random.Range(mapSize.x / 2, mapSize.x), 0);
                }
                else
                {
                    enemyEdge = mapEdges.RIGHT;
                    enemyTilePos = new Vector2Int(mapSize.x - 1, UnityEngine.Random.Range(0, mapSize.y / 2));
                }
                
                break;
            case mapQuarters.BOTTOM_LEFT:
                if (selectSide == 1)
                {
                    enemyEdge = mapEdges.TOP;
                    enemyTilePos = new Vector2Int(UnityEngine.Random.Range(mapSize.x / 2, mapSize.x), mapSize.y - 1);
                }
                else
                {
                    enemyEdge = mapEdges.RIGHT;
                    enemyTilePos = new Vector2Int(mapSize.x - 1, UnityEngine.Random.Range(mapSize.y / 2, mapSize.y));
                }
                
                break;
            case mapQuarters.BOTTOM_RIGHT:
                if (selectSide == 1)
                {
                    enemyEdge = mapEdges.TOP;
                    enemyTilePos = new Vector2Int(UnityEngine.Random.Range(0, mapSize.x / 2), mapSize.y - 1);
                }
                else
                {
                    enemyEdge = mapEdges.LEFT;
                    enemyTilePos = new Vector2Int(0, UnityEngine.Random.Range(mapSize.y / 2, mapSize.y));
                }
                
                break;
            default:
                break;
        }
    }
    
    private void generatePath()
    {
        path = createPath();
        
        foreach (var tile in path)
        {
            tilemapGround.SetTile(new Vector3Int(tile.x, tile.y, 0), tileWaypoint);
        }
    }
    
    private List<Vector2Int> createPath()
    {
        pathStartPos = baseTilePos;
        pathEndPos = enemyTilePos;

        List<Vector2Int> _path = new List<Vector2Int>();
        _path.Add(Vector2Int.zero);
        Vector2Int currentTile = pathStartPos;

        Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };

        int safety = 0;
        
        while (currentTile != pathEndPos && safety < 200)
        {
            safety++;
            
            Vector2Int preferredDir = Vector2Int.zero;
            
            if (Mathf.Abs(pathEndPos.x - currentTile.x) > Mathf.Abs(pathEndPos.y - currentTile.y))
            {
                preferredDir = (pathEndPos.x > currentTile.x) ? Vector2Int.right : Vector2Int.left;
            }
            else
            {
                preferredDir = (pathEndPos.y > currentTile.y) ? Vector2Int.up : Vector2Int.down;
            }

            Vector2Int moveDir = (Random.value < windingChance) ? dirs[Random.Range(0, dirs.Length)] : preferredDir;
            Vector2Int nextTile = currentTile + moveDir;

            if (_path.Contains(nextTile))
            {
                Debug.Log("Tile already occupied");
                
                for (int i = _path.Count - 1; i >= 0; i--)
                {
                    Debug.Log("Looking for last valid tile");
                    
                    if (freeNeighborCount(_path, i) <= 2)
                    {
                        _path.RemoveAt(i);
                        Debug.Log("Deleted tile at: " + i);
                    }
                    else if (freeNeighborCount(_path, i) > 2)
                    {
                        currentTile = _path[i];
                        break;
                    }
                }
            }
            else
            {
                _path.Add(nextTile);
                currentTile = nextTile;
            }
        }

        return _path;
    }
    
    private int freeNeighborCount(List<Vector2Int> tileList, int tileIndex)
    {
        int freeNeighbors = 0;
        
        if (!path.Contains(tileList[tileIndex] + Vector2Int.up))
        {
            freeNeighbors++;
        }
        if (!path.Contains(tileList[tileIndex] + Vector2Int.down))
        {
            freeNeighbors++;
        }
        if (!path.Contains(tileList[tileIndex] + Vector2Int.right))
        {
            freeNeighbors++;
        }
        if (!path.Contains(tileList[tileIndex] + Vector2Int.left))
        {
            freeNeighbors++;
        }

        return freeNeighbors;
    }
}