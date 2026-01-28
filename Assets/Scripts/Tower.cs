using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] TowerScriptableObject stats;
    [SerializeField] private List<Transform> targets = new List<Transform>();
    [SerializeField] private GameObject projectile;
    private Enemy newEnemy;

    private void Start()
    {
        CircleCollider2D trigger = GetComponent<CircleCollider2D>();
        trigger.radius = stats.range;

        StartCoroutine(fireProjectile());
    }

    private IEnumerator fireProjectile()
    {
        while (true)
        {
            if (targets.Count > 0)
            {
                GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.identity, transform);
                newProjectile.GetComponent<Projectile>().Initialize(targets[0], stats.damage);
                
                yield return new WaitForSeconds(1f / stats.attackspeed);
            }
            else
            {
                yield return null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((newEnemy = other.GetComponentInParent<Enemy>()) != null)
        {
            targets.Add(other.transform);
            newEnemy.OnEnemyDeath += Enemy_OnEnemyDeath;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        targets.Remove(collision.transform);
    }

    private void Enemy_OnEnemyDeath(object sender, Enemy.OnEnemyDeathEventArgs e)
    {
        targets.Remove(e.enemyObject.transform);
    }
}
