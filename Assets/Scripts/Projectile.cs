using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Transform target;

    private bool isInitialized = false;
    private Vector2 targetPos;

    public void Initialize(Transform _target)
    {
        target = _target;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;

        transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * 3f);
        transform.up = (target.position - transform.position).normalized;
    }
}
