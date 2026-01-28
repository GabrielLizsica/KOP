using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class BasicTrap : Trap
{
    [SerializeField] private BasicTrapScriptableObject stats;
    private Enemy targetEnemy;
    
    private void Start()
    {
        health = stats.health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((targetEnemy = collision.GetComponentInParent<Enemy>()) != null)
        {
            hitEnemy(targetEnemy, stats.damage);
            health--;
        }
    }
}
