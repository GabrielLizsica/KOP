using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class BasicTrap : Trap
{
    private void Start()
    {
        damage = stats.damage;
        health = stats.health;
        effectStrength = stats.effectstrength;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((targetEnemy = collision.GetComponentInParent<Enemy>()) != null)
        {
            hitEnemy();
        }
    }
}
