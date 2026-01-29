using UnityEngine;

public class PoisonTrap : Trap
{
    [SerializeField] private PoisonTrapScriptableObject stats;
    private Enemy targetEnemy;
    
    private void Start()
    {
        health = stats.health;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((targetEnemy = collision.GetComponentInParent<Enemy>()) != null)
        {
            hitEnemy(targetEnemy, stats.damage);
            applyEffect();
            health--;
        }
    }
    
    protected override void applyEffect()
    {
        targetEnemy.applyEffect(MainGameLogic.TrapEffects.POISON, stats.effectstrength, stats.effectduration);
    }
}
