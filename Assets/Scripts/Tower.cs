using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private List<Transform> targets;
    [SerializeField] private GameObject projectile;

    private void Start()
    {
        CircleCollider2D trigger = GetComponentInChildren<CircleCollider2D>();
        trigger.radius = range;

        StartCoroutine(fireProjectile());
    }

    private IEnumerator fireProjectile()
    {
        while (true)
        {
            if (targets.Count > 0)
            {
                GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.identity, transform);
                newProjectile.GetComponent<Projectile>().Initialize(targets[0]);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        targets.Add(other.transform);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        targets.Remove(other.transform);
    }
}
