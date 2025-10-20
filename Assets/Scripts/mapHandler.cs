using UnityEngine;
using UnityEngine.Tilemaps;

public class mapHandler : MonoBehaviour
{
    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private Tilemap tilemapGround;
    [SerializeField] private Tile tileEmpty;
    [SerializeField] private Tile tileBase;
    [SerializeField] private Tile tileEnemy;
    
    public Vector2Int MapSize
    {
        get{return mapSize;}
    }
    private Vector2Int baseTilePos;
    private Vector2Int enemyTilePos;
    private mapQuarters baseQuarter;
    
    private enum mapQuarters
    {
        DEFAULT,
        TOP_RIGHT,
        TOP_LEFT,
        BOTTOM_LEFT,
        BOTTOM_RIGHT
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
        
        baseTilePos = selectBaseTile();
        baseQuarter = getMapQuarter(baseTilePos);
        enemyTilePos = selectEnemyTile();

        tilemapGround.SetTile(new Vector3Int(baseTilePos.x, baseTilePos.y, 0), tileBase);
        tilemapGround.SetTile(new Vector3Int(enemyTilePos.x, enemyTilePos.y, 0), tileEnemy);
        Debug.Log("Base quarter: " + baseQuarter);
    }
    
    private mapQuarters getMapQuarter(Vector2Int tileChoords)
    {
        mapQuarters quarter = mapQuarters.DEFAULT;
        Vector2 adjustedChoords = new Vector2(tileChoords.x + 1, tileChoords.y + 1);
        
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
    
    private Vector2Int selectBaseTile()
    {
        Vector2Int _baseTilePos;
        
        int xPos = UnityEngine.Random.Range(0, mapSize.x);
        int yPos = UnityEngine.Random.Range(0, mapSize.y);
        
        _baseTilePos = new Vector2Int(xPos, yPos);
        
        return _baseTilePos;
    }
    
    private Vector2Int selectEnemyTile()
    {
        Vector2Int _enemyTilePos;
        
        if (baseQuarter == mapQuarters.TOP_RIGHT)
        {
            _enemyTilePos = new Vector2Int((int)UnityEngine.Random.Range(0, mapSize.x / 2), (int)UnityEngine.Random.Range(0, mapSize.y / 2));
        }
        else if (baseQuarter == mapQuarters.TOP_LEFT)
        {
            _enemyTilePos = new Vector2Int((int)UnityEngine.Random.Range(mapSize.x / 2, mapSize.x), (int)UnityEngine.Random.Range(0, mapSize.y / 2));
        }
        else if (baseQuarter == mapQuarters.BOTTOM_LEFT)
        {
            _enemyTilePos = new Vector2Int((int)UnityEngine.Random.Range(mapSize.x / 2, mapSize.x), (int)UnityEngine.Random.Range(mapSize.y / 2, mapSize.y));
        }
        else
        {
            _enemyTilePos = new Vector2Int((int)UnityEngine.Random.Range(0, mapSize.x / 2), (int)UnityEngine.Random.Range(mapSize.y / 2, mapSize.y));
        }

        return _enemyTilePos;
    }
}
