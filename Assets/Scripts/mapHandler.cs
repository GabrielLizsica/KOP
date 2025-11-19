using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Transactions;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapHandler : MonoBehaviour
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
    [SerializeField] private List<Vector2Int> path;

    public Vector2Int MapSize { get { return mapSize; } }
    public List<Vector2Int> Path { get { return path; } }
    public Vector2Int EnemyTilePos { get { return enemyTilePos; } }
    private Vector2Int baseTilePos;
    private Vector2Int enemyTilePos;
    private mapQuarters baseQuarter;
    private mapEdges enemyEdge;
    [SerializeField] private List<mapQuarters> occupiedQuarters = new List<mapQuarters>();
    
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
        occupiedQuarters.Add(baseQuarter);
        occupiedQuarters.Add(getMapQuarter(enemyTilePos));

        tilemapGround.SetTile(new Vector3Int(baseTilePos.x, baseTilePos.y, 0), tileBase);
        tilemapGround.SetTile(new Vector3Int(enemyTilePos.x, enemyTilePos.y, 0), tileEnemy);
        //Debug.Log("Base quarter: " + baseQuarter);

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
        List<Vector2Int> waypoints = createPathWaypoints();

        do
        {
            //Debug.LogWarning("Waypoints count: " + waypoints.Count);
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                //Debug.LogWarning("Waypoint " + i + ": " + waypoints[i]);
                createPath(waypoints[i], waypoints[i + 1]);
            }

            //Debug.LogWarning("Waypoints count: " + waypoints.Count);
            
            //if (path[path.Count - 1] != enemyTilePos) { Debug.LogError("Dead End - regenerating path!"); }
        } while (path[path.Count - 1] != baseTilePos);
        
        
        
        foreach (var tile in path)
        {
            tilemapGround.SetTile(new Vector3Int(tile.x, tile.y, 0), tileWaypoint);
        }
    }
    
    private void createPath(Vector2Int pathStartPos, Vector2Int pathEndPos)
    {
        Vector2Int currentTile = pathStartPos;
        Vector2Int nextTile  = new Vector2Int();
        path.Add(currentTile);
        int safety = 0;
        
        while (currentTile != pathEndPos && safety < 2000)
        {
            safety++;
            
            Vector2Int[] preferredDirs = new Vector2Int[2];
            Vector2Int wrongDir = Vector2Int.zero;

            if (pathEndPos.x != currentTile.x)
            {
                preferredDirs[0] = (pathEndPos.x > currentTile.x) ? Vector2Int.right : Vector2Int.left;
            }
            else
            {
                preferredDirs[0] = Vector2Int.zero;
            }
            
            if (pathEndPos.y != currentTile.y)
            {
                preferredDirs[1] = (pathEndPos.y > currentTile.y) ? Vector2Int.up : Vector2Int.down;
            }
            else
            {
                preferredDirs[1] = Vector2Int.zero;
            }
            
            List<Vector2Int> freeNeighbors = checkFreeDir(currentTile, preferredDirs);

            if (freeNeighbors.Count == 2)
            {
                //Debug.Log("Exactly 2 neighbors of tile: " + currentTile.x + ", " + currentTile.y);
                nextTile = freeNeighbors[Random.Range(0, 2)];
            }
            else if (freeNeighbors.Count == 1)
            {
                //Debug.Log("Exactly 1 neighbor of tile: " + currentTile.x + ", " + currentTile.y);
                nextTile = freeNeighbors[0];
            }
            else if (freeNeighbors.Count == 0)
            {
                Vector2Int[] dirs = { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };
                freeNeighbors = checkFreeDir(currentTile, dirs);
                
                if (freeNeighbors.Count > 0)
                {
                    nextTile = freeNeighbors[Random.Range(0, freeNeighbors.Count)];
                }
                else
                {
                    //Debug.LogWarning("Dead end. Cannot continue path.");
                    break;
                }
            }

            path.Add(nextTile);
            currentTile = nextTile;
        }

        //Debug.LogWarning("Finished generation");
    }

    private List<Vector2Int> createPathWaypoints()
    {
        List<Vector2Int> _waypoints = new List<Vector2Int>();
        _waypoints.Add(enemyTilePos);
        
        if (!occupiedQuarters.Contains(mapQuarters.TOP_RIGHT))
        {
            _waypoints.Add(placePathWaypoint(mapQuarters.TOP_RIGHT));
        }
        if (!occupiedQuarters.Contains(mapQuarters.TOP_LEFT))
        {
            _waypoints.Add(placePathWaypoint(mapQuarters.TOP_LEFT));
        }
        if (!occupiedQuarters.Contains(mapQuarters.BOTTOM_LEFT))
        {
            _waypoints.Add(placePathWaypoint(mapQuarters.BOTTOM_LEFT));
        }
        if (!occupiedQuarters.Contains(mapQuarters.BOTTOM_RIGHT))
        {
            _waypoints.Add(placePathWaypoint(mapQuarters.BOTTOM_RIGHT));
        }

        _waypoints.Add(baseTilePos);
        return _waypoints;
    }
    
    private Vector2Int placePathWaypoint(mapQuarters quarter)
    {
        Vector2Int waypoint = new Vector2Int();
        
        if (quarter == mapQuarters.TOP_RIGHT)
        {
            waypoint = new Vector2Int(Random.Range(mapSize.x / 2, mapSize.x), Random.Range(mapSize.y / 2, mapSize.y));
        }
        else if (quarter == mapQuarters.TOP_LEFT)
        {
            waypoint = new Vector2Int(Random.Range(0, mapSize.x / 2), Random.Range(mapSize.y / 2, mapSize.y));
        }
        else if (quarter == mapQuarters.BOTTOM_LEFT)
        {
            waypoint = new Vector2Int(Random.Range(0, mapSize.x / 2), Random.Range(0, mapSize.y / 2));
        }
        else if (quarter == mapQuarters.BOTTOM_RIGHT)
        {
            waypoint = new Vector2Int(Random.Range(mapSize.x / 2, mapSize.x), Random.Range(0, mapSize.y / 2));
        }

        return waypoint;
    }
    
    private List<Vector2Int> checkFreeDir(Vector2Int currentTile, Vector2Int[] dirs)
    {
        List<Vector2Int> freeNeighbors = new List<Vector2Int>();
        Vector2Int checkNeighbor;
        
        foreach (Vector2Int dir in dirs)
        {
            if (dir != Vector2Int.zero)
            {
                checkNeighbor = currentTile + dir;
            
                if (!path.Contains(checkNeighbor) && !checkOutOfBounds(checkNeighbor) && tileHasEscape(checkNeighbor))
                {
                    freeNeighbors.Add(checkNeighbor);
                }
            }
        }

        return freeNeighbors;
    }

    private bool checkOutOfBounds(Vector2Int tile)
    {
        if (tile.x > mapSize.x - 1 || tile.y > mapSize.y - 1 || tile.x < 0 || tile.y < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    private bool tileHasEscape(Vector2Int tile)
    {
        int escapes = 0;
        
        if (!path.Contains(tile + Vector2Int.up) && !checkOutOfBounds(tile + Vector2Int.up))
        {
            escapes++;
        }
        if (!path.Contains(tile + Vector2Int.down) && !checkOutOfBounds(tile + Vector2Int.down))
        {
            escapes++;
        }
        if (!path.Contains(tile + Vector2Int.right) && !checkOutOfBounds(tile + Vector2Int.right))
        {
            escapes++;
        }
        if (!path.Contains(tile + Vector2Int.left) && !checkOutOfBounds(tile + Vector2Int.left))
        {
            escapes++;
        }
        
        if (escapes >= 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    /*
    private List<Vector2Int> backtrackPath(ref List<Vector2Int> list, ref Vector2Int currentTile)
    {
        List<Vector2Int> freeNeighbors = new List<Vector2Int>();
        Vector2Int badTile = new Vector2Int();
    
        for (int i = list.Count - 1; i >= 0; i--)
        {
            freeNeighbors = checkFreeDir(list[i], list, new Vector2Int(-1, -1));
            //Debug.Log("Free neighbors of tile: " + list[i].x + ", " + list[i].y + " --> " + freeNeighbors.Count);
            
            if (freeNeighbors.Count < 2)
            {
                badTile = list[i];
                //Debug.Log("removed bad tile at: " + list[i].x + ", " + list[i].y + " (i = " + i + " )");
                list.RemoveAt(i);
            }
            else
            {
                //Debug.Log("Found good tile at: " + list[i].x + ", " + list[i].y + " (i = " + i + " ) - Breaking backtrack");
                currentTile = list[i];
                break;
            }
        }
        
        for (int i = 0; i < freeNeighbors.Count; i++)
        {
            if (freeNeighbors.Contains(badTile))
            {
                freeNeighbors.Remove(badTile);
            }
        }

        return freeNeighbors;
    }
    */
}