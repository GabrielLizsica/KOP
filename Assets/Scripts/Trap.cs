using UnityEngine;

public  class Trap : MonoBehaviour
{
    [SerializeField] protected TrapScriptableObject stats;
    [SerializeField] protected int damage;
    [SerializeField] protected int health;
    [SerializeField] protected float effectStrength;

    protected Enemy targetEnemy;
    
    protected void hitEnemy()
    {
        targetEnemy.health -= damage * targetEnemy.weakness;
    }

    protected virtual void ApplyEffect() {}
}
