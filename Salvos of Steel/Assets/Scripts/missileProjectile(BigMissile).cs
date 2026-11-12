using UnityEngine;
using System.Collections;

public class MissileProjectile : MonoBehaviour
{
    [Header("Missile Settings")]
    public float speed = 0.01f;
    public float damage = 6000f;
    public float lifetime = 10f;

    [Header("Homing Settings")]
    [Tooltip("Maximum rotation speed in degrees per second.")]
    public float turnSpeed = 60f;

    private Transform target;

    private bool canDamageHelicopter = false;
    private bool canDamagePlayer = false;

    // Separate disruption states.
    private bool chaffDisabled = false;
    private bool smokeDisabled = false;
    private bool permanentlyDisabled = false;

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

        // Check whether the target is inside active smoke.
        smokeDisabled =
            SmokeScreenArea.IsPointInsideActiveSmoke(target.position);

        bool shouldDisableGuidance =
            chaffDisabled ||
            smokeDisabled ||
            permanentlyDisabled;

        // Capture the current direction when guidance first stops.
        if (shouldDisableGuidance && !guidanceDisabled)
        {
            unguidedDirection = transform.up.normalized;
        }

        guidanceDisabled = shouldDisableGuidance;

        if (guidanceDisabled)
        {
            transform.up = unguidedDirection;
        }
        else
        {
            Vector2 directionToTarget =
                (Vector2)target.position -
                (Vector2)transform.position;

            if (directionToTarget.sqrMagnitude > 0.0001f)
            {
                directionToTarget.Normalize();

                float currentAngle =
                    Mathf.Atan2(transform.up.y, transform.up.x)
                    * Mathf.Rad2Deg;

                float targetAngle =
                    Mathf.Atan2(directionToTarget.y, directionToTarget.x)
                    * Mathf.Rad2Deg;

                float newAngle = Mathf.MoveTowardsAngle(
                    currentAngle,
                    targetAngle,
                    turnSpeed * Time.deltaTime
                );

                float radians = newAngle * Mathf.Deg2Rad;

                Vector2 newDirection = new Vector2(
                    Mathf.Cos(radians),
                    Mathf.Sin(radians)
                );

                transform.up = newDirection;
            }
        }

        // The missile always continues moving forward.
        transform.position +=
            (Vector3)(transform.up * speed * Time.deltaTime);
    }

    public bool IsTargeting(Transform possibleTarget)
    {
        return target == possibleTarget;
    }

    // Existing temporary chaff disruption.
    public void DisableGuidance(float duration)
    {
        if (!chaffDisabled && !guidanceDisabled)
        {
            unguidedDirection = transform.up.normalized;
        }

        chaffDisabled = true;

        if (guidanceCoroutine != null)
        {
            StopCoroutine(guidanceCoroutine);
        }

        guidanceCoroutine = StartCoroutine(
            RestoreGuidance(duration)
        );
    }

    private IEnumerator RestoreGuidance(float duration)
    {
        yield return new WaitForSeconds(duration);

        chaffDisabled = false;
        guidanceCoroutine = null;

        Debug.Log("BIG MISSILE CHAFF DISRUPTION ENDED!");
    }

    // New permanent interception disruption.
    public void PermanentlyDisableGuidance()
    {
        if (permanentlyDisabled)
            return;

        if (!guidanceDisabled)
        {
            unguidedDirection = transform.up.normalized;
        }

        permanentlyDisabled = true;
        guidanceDisabled = true;

        Debug.Log("MISSILE PERMANENTLY INTERCEPTED: " + name);
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
