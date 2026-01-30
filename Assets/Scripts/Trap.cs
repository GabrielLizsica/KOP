using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public  class Trap : MonoBehaviour
{
    [SerializeField] protected int health;
    [SerializeField] protected float damage;
    
    protected void hitEnemy(Enemy targetEnemy, float damage)
    {
        targetEnemy.health -= damage * targetEnemy.weakness;
    }

    protected virtual void applyEffectTarget() {}
    public virtual void applyEffect(MainGameLogic.CardTypes effect, float effectStrength, float effectDuration) {}
}
