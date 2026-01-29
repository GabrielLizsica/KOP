using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public  class Trap : MonoBehaviour
{
    [SerializeField] protected int health;
    
    protected void hitEnemy(Enemy targetEnemy, int damage)
    {
        targetEnemy.health -= damage * targetEnemy.weakness;
    }

    protected virtual void applyEffect() {}
}
