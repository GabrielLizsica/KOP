using UnityEngine;

public class waveHandler : MonoBehaviour
{
    [SerializeField] private GameObject enemy;

    [SerializeField] private mapHandler mapHandler;
    
    private void Start()
    {
        mapHandler = GetComponent<mapHandler>();
    }
    
    public void spawnWave(int waveCount, int enemyCount)
    {
        for (int i = 0; i < waveCount; i++)
        {
            for (int j = 0; j < enemyCount; j++)
            {
                Instantiate(enemy, new Vector3Int(mapHandler.EnemyTilePos.x, mapHandler.EnemyTilePos.y, 1), Quaternion.identity).name = "Enemy" + j;
            }
        }
    }
}
