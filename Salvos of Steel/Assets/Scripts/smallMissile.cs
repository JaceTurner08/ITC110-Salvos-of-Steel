
using UnityEngine;
using System.Collections;

public class smallMissile : MonoBehaviour
{
    [Header("Missile Settings")]
    public float speed = 0.05f;
    [Header("Homing Settings")]
public float turnSpeed = 90f;
    public float damage = 4500f;
    public float lifetime = 10f;

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

        if (!guidanceDisabled)
        {
            Vector2 direction =
                (Vector2)target.position - (Vector2)transform.position;

            transform.up = direction;

            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );
        }
        else
        {
            // Fly straight while guidance is disrupted.
            transform.up = unguidedDirection;

            transform.position +=
                (Vector3)(unguidedDirection * speed * Time.deltaTime);
        }
    }

    public bool IsTargeting(Transform possibleTarget)
    {
        return target == possibleTarget;
    }

    public void DisableGuidance(float duration)
    {
        if (!guidanceDisabled)
        {
            if (target != null)
            {
                unguidedDirection =
                    ((Vector2)target.position - (Vector2)transform.position).normalized;
            }
            else
            {
                unguidedDirection = transform.up;
            }
        }

        guidanceDisabled = true;

        if (guidanceCoroutine != null)
            StopCoroutine(guidanceCoroutine);

        guidanceCoroutine = StartCoroutine(RestoreGuidance(duration));
    }

    private IEnumerator RestoreGuidance(float duration)
    {
        yield return new WaitForSeconds(duration);

        guidanceDisabled = false;
        guidanceCoroutine = null;

        Debug.Log("Small missile has reacquired its target!");
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
