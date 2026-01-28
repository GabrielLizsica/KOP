using UnityEngine;

public class IceTrap : Trap
{
    [SerializeField] private IceTrapScriptableObject stats;
    [SerializeField] private MainGameLogic mainGameLogic;
    private Enemy targetEnemy;
    
    private void Start()
    {
        mainGameLogic = FindAnyObjectByType<MainGameLogic>();
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
        targetEnemy.applyEffect(MainGameLogic.TrapEffects.ICE, stats.effectstrength, stats.effectduration);
    }
}
