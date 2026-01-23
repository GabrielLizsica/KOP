using UnityEngine;

public class IceTrap : Trap
{
    [SerializeField] private IceTrapScriptableObject stats;
    [SerializeField] private MainGameLogic mainGameLogic;
    private Enemy targetEnemy;
    
    private void Start()
    {
        mainGameLogic = FindAnyObjectByType<MainGameLogic>();
        damage = stats.damage;
        health = stats.health;
        effectStrength = stats.effectstrength;
        effectDuration = stats.effectduration;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((targetEnemy = collision.GetComponentInParent<Enemy>()) != null)
        {
            hitEnemy(targetEnemy);
            applyEffect();
            health--;
        }
    }
    
    protected override void applyEffect()
    {
        targetEnemy.applyEffect(MainGameLogic.TrapEffects.ICE, effectStrength, effectDuration);
    }
}
