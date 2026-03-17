using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class BasicTrap : Trap
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
            health--;
        }
    }
    
    //Applies selected effect onto this object
    public override void applyEffect(MainGameLogic.CardTypes effect, float effectStrength, float effectDuration) 
    {
        if (effect == MainGameLogic.CardTypes.DAMAGE_BUFF)
        {
            damage = damage * (1 + effectStrength / 100);
        }

        StartCoroutine(resetEffect(effectDuration));
    }
    
    //Resets effects on this object
    private IEnumerator resetEffect(float duration)
    {
        yield return new WaitForSeconds(duration);

        damage = stats.damage;
    }
}
