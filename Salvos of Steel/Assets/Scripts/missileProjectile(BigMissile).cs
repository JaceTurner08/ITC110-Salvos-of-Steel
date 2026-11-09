using UnityEngine;
using System.Collections;

public class MissileProjectile : MonoBehaviour
{
    [Header("Missile Settings")]
    public float speed = 0.01f;
    [Header("Homing Settings")]
    public float turnSpeed = 90f;
    public float damage = 6000f;
    public float lifetime = 10f;

    private Transform target;
    private bool canDamageHelicopter = false;
    private bool canDamagePlayer = false;

    private bool guidanceDisabled = false;
    private Vector2 unguidedDirection;
    private Coroutine guidanceCoroutine;

    public void SetTarget(
        Transform newTarget,
        float missileDamage,
        bool allowHelicopterDamage = false,
        bool allowPlayerDamage = false)
    {
        target = newTarget;
        damage = missileDamage;
        canDamageHelicopter = allowHelicopterDamage;
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
            // Continue flying in the direction the missile was already travelling.
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
        // Preserve the missile's current flight direction.
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

        // Restart the disruption timer if guidance is disabled again.
        if (guidanceCoroutine != null)
            StopCoroutine(guidanceCoroutine);

        guidanceCoroutine = StartCoroutine(RestoreGuidance(duration));
    }

    private IEnumerator RestoreGuidance(float duration)
    {
        yield return new WaitForSeconds(duration);

        guidanceDisabled = false;
        guidanceCoroutine = null;

        Debug.Log("Big missile has reacquired its target!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (canDamageHelicopter)
        {
            HelicopterHealth helicopter =
                other.GetComponentInParent<HelicopterHealth>();

            if (helicopter != null)
            {
                helicopter.TakeSmallMissileHit();
                Destroy(gameObject);
                return;
            }
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
