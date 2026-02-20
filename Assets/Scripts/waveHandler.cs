using System;
using System.Collections;
using UnityEngine;

public class WaveHandler : MonoBehaviour
{
    [SerializeField] private GameObject battleUIObject;
    [SerializeField] private GameObject enemyObject;

    [SerializeField] private MapHandler mapHandler;

    [SerializeField] private Enemy enemy;
    private MainGameLogic mainGameLogic;
    private InBattleMenuHandler battleUI;
    public int aliveEnemies { private set; get; }
    public int remainingEnemies { private set; get; }
    
    private void Start()
    {
        mapHandler = GetComponent<MapHandler>();
        mainGameLogic = GetComponent<MainGameLogic>();
        battleUI = battleUIObject.GetComponent<InBattleMenuHandler>();
        aliveEnemies = 1;
    }
    
    public IEnumerator spawnWave(int waveCount, int enemyCount, float timeBetweenWaves, float timeBetweenenemies)
    {
        remainingEnemies = waveCount * enemyCount;

        for (int i = 0; i < waveCount; i++)
        {
            GameObject newWave = new GameObject($"Wave{i}");
            
            for (int j = 0; j < enemyCount; j++)
            {
                GameObject spawnedEnemy = Instantiate(enemyObject, new Vector3Int(mapHandler.EnemyTilePos.x, mapHandler.EnemyTilePos.y, 0), Quaternion.identity, newWave.transform);
                spawnedEnemy.name = "Enemy" + i + "_" + j;
                enemy = spawnedEnemy.GetComponent<Enemy>();
                enemy.OnBaseReached += Enemy_OnBasereached;
                enemy.OnEnemyDeath += Enemy_OnEnemyDeath;

                aliveEnemies++;
                remainingEnemies--;
                yield return new WaitForSeconds(timeBetweenenemies);
            }
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }
    
    private void Enemy_OnBasereached(object sender, Enemy.OnBaseReachedEventArgs e)
    {
        mainGameLogic.currentHealth -= 25f;
        aliveEnemies--;
        Transform enemyParent = e.enemyObject.transform.parent;
        Destroy(e.enemyObject.gameObject);
        
        battleUI.updateLabel(InBattleMenuHandler.displayLabels.HEALTH);
        StartCoroutine(DeleteWaveEmpty(enemyParent));
    }

    private void Enemy_OnEnemyDeath(object sender, Enemy.OnEnemyDeathEventArgs e)
    {
        mainGameLogic.currentEnergy += 20;
        battleUI.updateLabel(InBattleMenuHandler.displayLabels.ENERGY);
        aliveEnemies--;
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
