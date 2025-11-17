using UnityEngine;

public class waveHandler : MonoBehaviour
{
    [SerializeField] private GameObject enemy;
    public void spawnWave(int waveCount, int enemyCount)
    {
        for (int i = 0; i < waveCount; i++)
        {
            for (int j = 0; j < enemyCount; j++)
            {
                Instantiate(enemy, new Vector3Int(0, 0, 0), Quaternion.identity).name = "Enemy" + j;
            }
        }
    }
}
