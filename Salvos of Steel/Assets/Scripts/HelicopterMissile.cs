using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HelicopterMissile : MonoBehaviour
{
    [Header("Missile Settings")]
    public float speed = 8f;
    public float damage = 1500f;
    public float lifetime = 8f;

    private Transform target;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 direction =
            (Vector2)target.position - rb.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.up = direction;

            Vector2 newPosition = Vector2.MoveTowards(
                rb.position,
                target.position,
                speed * Time.fixedDeltaTime
            );

            rb.MovePosition(newPosition);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemy =
            other.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            // Damage the enemy.
            enemy.TakeDamage(damage);

            // Destroy the missile immediately.
            Destroy(gameObject);
        }
    }
}
