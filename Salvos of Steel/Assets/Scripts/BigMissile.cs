using UnityEngine;

public class MissileProjectile : MonoBehaviour
{
    public float speed = 0.01f;
    public float damage = 8000f;
    public float lifetime = 10f;

    private Transform target;

    public void SetTarget(Transform newTarget, float missileDamage)
    {
        target = newTarget;
        damage = missileDamage;
    }

    void Start()
    {
        // Destroy the missile if it takes too long to reach its target.
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // If the enemy no longer exists, destroy the missile.
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move the missile toward the enemy.
        Vector2 direction = (Vector2)target.position - (Vector2)transform.position;

        transform.up = direction;

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check whether the missile collided with an enemy.
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            // Apply damage to the enemy.
            enemy.TakeDamage(damage);

            // Immediately remove the missile from the scene.
            Destroy(gameObject);
        }
    }
}
