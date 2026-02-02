using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.DualShock.LowLevel;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MapHandler mapHandler;
    [SerializeField] private MainGameLogic mainGameLogic;

    [Header("Enemy variables")]
    [SerializeField] private EnemyScriptableObject stats;
    [SerializeField] private List<Vector2Int> path;
    [SerializeField] private float speed;
    [SerializeField] public float health;
    [SerializeField] public float weakness;

    public event EventHandler<OnBaseReachedEventArgs> OnBaseReached;
    public event EventHandler<OnEnemyDeathEventArgs> OnEnemyDeath;
    public class OnBaseReachedEventArgs : EventArgs
    {
        public GameObject enemyObject;
    }

    public class OnEnemyDeathEventArgs : EventArgs
    {
        public GameObject enemyObject;
    }

    private Vector3 targetTile;
    private int targetIndex = 0;
    private int posX;
    private int posY;

    private void Start()
    {
        if (stats == null)
        {
            Application.Quit();
        }
        speed = stats.speed;
        health = stats.health;
        weakness = stats.weakness;
        
        mainGameLogic = FindAnyObjectByType<MainGameLogic>();
        mapHandler = FindAnyObjectByType<MapHandler>();
        path = mapHandler.Path;
    }

    private void Update()
    {
        if (health <= 0)
        {
            OnEnemyDeath?.Invoke(this, new OnEnemyDeathEventArgs { enemyObject = transform.gameObject });
            Destroy(gameObject);
            
        }

        if (Vector3.Distance(transform.position, new Vector3(path[path.Count - 1].x + 0.5f, path[path.Count - 1].y + 0.5f, 0)) > 0.1f)
        {
            targetTile = new Vector3(path[targetIndex].x + 0.5f, path[targetIndex].y + 0.5f, 0);

            if (Vector3.Distance(transform.position, targetTile) < 0.1f)
            {
                targetIndex++;
            }

            moveAlongPath(targetTile);
        }
        else
        {
            OnBaseReached?.Invoke(this, new OnBaseReachedEventArgs { enemyObject = transform.gameObject });
        }
    }

    private void moveAlongPath(Vector3 targetPos)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
    }

    public void applyEffect(MainGameLogic.TrapEffects effect, float effectStrength, float duration)
    {
        if (effect == MainGameLogic.TrapEffects.ICE)
        {
            speed = speed * (1 - effectStrength / 100);
        }
        else if (effect == MainGameLogic.TrapEffects.POISON)
        {
            weakness = weakness * (1 + effectStrength / 100);
        }

        StartCoroutine(resetEffect(duration));
    }
    
    private IEnumerator resetEffect(float duration)
    {
        yield return new WaitForSeconds(duration);

        speed = stats.speed;
        weakness = stats.weakness;
    }
}
