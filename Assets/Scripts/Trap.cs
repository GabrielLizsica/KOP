using UnityEngine;

public  class Trap : MonoBehaviour
{
    [SerializeField] protected int damage;
    [SerializeField] protected int health;
    [SerializeField] protected float effectStrength;
    [SerializeField] protected float effectDuration;
    
    
    protected void hitEnemy(Enemy targetEnemy)
    {
        targetEnemy.health -= damage * targetEnemy.weakness;
    }

    protected virtual void applyEffect() {}
}
