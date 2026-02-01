using System;
using System.Collections;
using UnityEngine;

public class WaveHandler : MonoBehaviour
{
    [SerializeField] private GameObject enemyObject;

    [SerializeField] private MapHandler mapHandler;

    [SerializeField] private Enemy enemy;
    private MainGameLogic mainGameLogic;
    
    private void Start()
    {
        mapHandler = GetComponent<MapHandler>();
        mainGameLogic = GetComponent<MainGameLogic>();
    }
    
    public IEnumerator spawnWave(int waveCount, int enemyCount, float timeBetweenWaves, float timeBetweenenemies)
    {
        for (int i = 0; i < waveCount; i++)
        {
            GameObject newWave = new GameObject($"Wave{i}");
            
            for (int j = 0; j < enemyCount; j++)
            {
                GameObject spawnedEnemy = Instantiate(enemyObject, new Vector3Int(mapHandler.EnemyTilePos.x, mapHandler.EnemyTilePos.y, 0), Quaternion.identity, newWave.transform);
                spawnedEnemy.name = "Enemy" + i + "_" + j;
                enemy = spawnedEnemy.GetComponent<Enemy>();
                enemy.OnBaseReached += Enemy_OnBasereached;

                yield return new WaitForSeconds(timeBetweenenemies);
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }
    
    private void Enemy_OnBasereached(object sender, Enemy.OnBaseReachedEventArgs e)
    {
        mainGameLogic.baseHealth -= 25f;
        Transform enemyParent = e.enemyObject.transform.parent;
        Destroy(e.enemyObject.gameObject);
        StartCoroutine(DeleteWaveEmpty(enemyParent));
    }
    
    private IEnumerator DeleteWaveEmpty(Transform parent)
    {
        yield return null;
        
        if (parent.childCount == 0)
        {
            Destroy(parent.gameObject);
        }
    }
}
