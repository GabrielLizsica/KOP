using UnityEngine;

public class WaveHandler : MonoBehaviour
{
    [SerializeField] private GameObject enemy;

    [SerializeField] private MapHandler mapHandler;
    
    private void Start()
    {
        mapHandler = GetComponent<MapHandler>();
    }
    
    public void spawnWave(int waveCount, int enemyCount)
    {
        for (int i = 0; i < waveCount; i++)
        {
            for (int j = 0; j < enemyCount; j++)
            {
                Instantiate(enemy, new Vector3Int(mapHandler.EnemyTilePos.x, mapHandler.EnemyTilePos.y, 0), Quaternion.identity).name = "Enemy" + j;
            }
        }
    }
}
