using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class mapHandler : MonoBehaviour
{
    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private int waypointCount;
    [SerializeField] private Tilemap tilemapGround;
    [SerializeField] private Tile tileEmpty;
    [SerializeField] private Tile tileBase;
    [SerializeField] private Tile tileEnemy;
    [SerializeField] private Tile tileWaypoint;
    
    
    public Vector2Int MapSize
    {
        get{return mapSize;}
    }
    private Vector2Int baseTilePos;
    private Vector2Int enemyTilePos;
    private mapQuarters baseQuarter;
    private mapEdges enemyEdge;

    private List<Vector2Int> waypoints = new List<Vector2Int>();
    
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

        createPath();
        
        for (int i = 0; i < waypointCount; i++)
        {
            tilemapGround.SetTile(new Vector3Int(waypoints[i].x, waypoints[i].y, 0), tileWaypoint);
        }
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
    
    private void createPath()
    {
        float mainDistance = Vector2Int.Distance(baseTilePos, enemyTilePos);
        float lastDistance = mainDistance;

        for (int i = 0; i < waypointCount; i++)
        {
            waypoints.Add(createWaypoint(mainDistance, ref lastDistance));
            Debug.Log("Next waypoint: " + waypoints[i].x + ", " + waypoints[i].y);
        }
    }
    
    private Vector2Int createWaypoint(float mainDistance, ref float lastDistance)
    {
        Vector2Int nextWaypoint = new Vector2Int();
        float baseDistance, enemyDistance;
        bool validPoint;

        do
        {
            nextWaypoint = new Vector2Int(UnityEngine.Random.Range(0, mapSize.x), UnityEngine.Random.Range(0, mapSize.y));

            baseDistance = Vector2Int.Distance(baseTilePos, nextWaypoint);
            enemyDistance = Vector2Int.Distance(enemyTilePos, nextWaypoint);
            
            if (baseDistance < mainDistance && enemyDistance < mainDistance)
            {
                lastDistance = Vector2Int.Distance(baseTilePos, nextWaypoint);
                validPoint = true;
            }
            else
            {
                validPoint = false;
            }
        } while (!validPoint);

        return nextWaypoint;
    }
}