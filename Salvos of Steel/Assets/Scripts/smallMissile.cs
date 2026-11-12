
using UnityEngine;
using System.Collections;

public class smallMissile : MonoBehaviour
{
    [Header("Missile Settings")]
    public float speed = 0.05f;
    public float damage = 4500f;
    public float lifetime = 10f;

    [Header("Homing Settings")]
    [Tooltip("Maximum rotation speed in degrees per second.")]
    public float turnSpeed = 100f;

    private Transform target;
    private bool canDamagePlayer = false;

    private bool guidanceDisabled = false;
    private Vector2 unguidedDirection;

    private Coroutine guidanceCoroutine;

    public void SetTarget(
        Transform newTarget,
        float missileDamage,
        bool allowPlayerDamage = false)
    {
        target = newTarget;
        damage = missileDamage;
        canDamagePlayer = allowPlayerDamage;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        if (guidanceDisabled)
        {
            // Continue flying straight during chaff disruption.
            transform.up = unguidedDirection;
        }
        else
        {
            // Calculate direction toward the target.
            Vector2 directionToTarget =
                (Vector2)target.position -
                (Vector2)transform.position;

            if (directionToTarget.sqrMagnitude > 0.0001f)
            {
                directionToTarget.Normalize();

                // Calculate current and desired angles.
                float currentAngle =
                    Mathf.Atan2(transform.up.y, transform.up.x)
                    * Mathf.Rad2Deg;

                float targetAngle =
                    Mathf.Atan2(directionToTarget.y, directionToTarget.x)
                    * Mathf.Rad2Deg;

                // Gradually rotate toward the target.
                float newAngle = Mathf.MoveTowardsAngle(
                    currentAngle,
                    targetAngle,
                    turnSpeed * Time.deltaTime
                );

                // Apply the new rotation.
                float radians = newAngle * Mathf.Deg2Rad;

                Vector2 newDirection = new Vector2(
                    Mathf.Cos(radians),
                    Mathf.Sin(radians)
                );

                transform.up = newDirection;
            }
        }

        // Continue moving forward.
        transform.position +=
            (Vector3)(transform.up * speed * Time.deltaTime);
    }

    public bool IsTargeting(Transform possibleTarget)
    {
        return target == possibleTarget;
    }

    public void DisableGuidance(float duration)
    {
        if (!guidanceDisabled)
        {
            unguidedDirection = transform.up.normalized;
        }

        guidanceDisabled = true;

        if (guidanceCoroutine != null)
        {
            StopCoroutine(guidanceCoroutine);
        }

        guidanceCoroutine = StartCoroutine(RestoreGuidance(duration));
    }

    private IEnumerator RestoreGuidance(float duration)
    {
        yield return new WaitForSeconds(duration);

        guidanceDisabled = false;
        guidanceCoroutine = null;

        Debug.Log("SMALL MISSILE GUIDANCE RESTORED!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemy =
            other.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (canDamagePlayer)
        {
            PlayerHealth player =
                other.GetComponentInParent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(Mathf.RoundToInt(damage));
                Destroy(gameObject);
            }
        }
    }
}
