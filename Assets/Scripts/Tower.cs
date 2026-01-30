using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private TowerScriptableObject stats;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float damage;
    [SerializeField] private List<Transform> targets = new List<Transform>();
    [SerializeField] private GameObject projectile;
    private CircleCollider2D trigger;
    private Enemy newEnemy;

    private void Start()
    {
        trigger = GetComponent<CircleCollider2D>();
        trigger.radius = stats.range;
        
        attackSpeed = stats.attackspeed;
        damage = stats.damage;

        StartCoroutine(fireProjectile());
    }

    private IEnumerator fireProjectile()
    {
        while (true)
        {
            if (targets.Count > 0)
            {
                GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.identity, transform);
                newProjectile.GetComponent<Projectile>().Initialize(targets[0], damage);
                
                yield return new WaitForSeconds(1f / attackSpeed);
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
    
    public void applyEffect(MainGameLogic.CardTypes effect, float effectStrength, float effectDuration)
    {
        if (effect == MainGameLogic.CardTypes.ATTACK_SPEED_BUFF)
        {
            attackSpeed = attackSpeed * (1 + effectStrength / 100);
        }
        else if (effect == MainGameLogic.CardTypes.RANGE_BUFF)
        {
            trigger.radius = trigger.radius * (1 + effectStrength / 100); ;
        }
        else if (effect == MainGameLogic.CardTypes.DAMAGE_BUFF)
        {
            damage = damage * (1 + effectStrength / 100); ;
        }

        StartCoroutine(resetEffect(effectDuration));
    }
    
    public IEnumerator resetEffect(float duration)
    {
        yield return new WaitForSeconds(duration);
        
        attackSpeed = stats.attackspeed;
        trigger.radius = stats.range;
        damage = stats.damage;
    }
}
