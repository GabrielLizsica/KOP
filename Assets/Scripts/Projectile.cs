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

    public void Initialize(Transform _target, float _damage)
    {
        damage = _damage;
        target = _target;
        targetEnemy = target.gameObject.GetComponent<Enemy>();
        bodyTransform = gameObject.GetComponentInChildren<Transform>();
        
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;
        //if (target == null) Destroy(gameObject);

        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * travelSpeed);
            bodyTransform.up = (target.position - transform.position).normalized;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void hitEnemy()
    {
        targetEnemy.health -= damage * targetEnemy.weakness;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == target)
        {
            hitEnemy();
        }
    }
}
