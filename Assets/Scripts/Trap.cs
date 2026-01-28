using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public  class Trap : MonoBehaviour
{
    [SerializeField] protected int health;
    [SerializeField] protected float effectStrength;
    [SerializeField] protected float effectDuration;
    
    
    protected void hitEnemy(Enemy targetEnemy, int damage)
    {
        targetEnemy.health -= damage * targetEnemy.weakness;
    }

    protected virtual void applyEffect() {}
}
