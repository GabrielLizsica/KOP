using System.Collections;
using UnityEngine;

public class IceTrap : Trap
{
    [SerializeField] private TrapScriptableObject stats;
    private Enemy targetEnemy;
    
    private void Start()
    {
        health = stats.health;
        damage = stats.damage;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((targetEnemy = collision.GetComponentInParent<Enemy>()) != null)
        {
            hitEnemy(targetEnemy, damage);
            applyEffectTarget();
            health--;
        }
    }
    
    protected override void applyEffectTarget()
    {
        targetEnemy.applyEffect(MainGameLogic.TrapEffects.ICE, stats.effectstrength, stats.effectduration);
    }
    
    public override void applyEffect(MainGameLogic.CardTypes effect, float effectStrength, float effectDuration) 
    {
        if (effect == MainGameLogic.CardTypes.DAMAGE_BUFF)
        {
            damage = damage * (1 + effectStrength / 100);
        }

        StartCoroutine(resetEffect(effectDuration));
    }
    
    private IEnumerator resetEffect(float duration)
    {
        yield return new WaitForSeconds(duration);

        damage = stats.damage;
    }
}
