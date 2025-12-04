using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float travelSpeed;
    [SerializeField] private float damage;
    private Transform bodyTransform;
    private Enemy targetEnemy;

    private bool isInitialized = false;
    private Vector2 targetPos;

    public void Initialize(Transform _target)
    {
        target = _target;
        targetEnemy = target.gameObject.GetComponent<Enemy>();
        isInitialized = true;
        bodyTransform = gameObject.GetComponentInChildren<Transform>();
    }

    private void Update()
    {
        if (!isInitialized) return;
        if (target == null) Destroy(gameObject);
        
        if(Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            hitEnemy();
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * travelSpeed);
            bodyTransform.up = (target.position - transform.position).normalized;
        }
    }
    
    private void hitEnemy()
    {
        targetEnemy.health -= damage * targetEnemy.weakness;
        Destroy(gameObject);
    }
}
