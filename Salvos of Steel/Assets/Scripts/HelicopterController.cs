using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HelicopterController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;

    [Header("Targeting")]
    public float targetSearchInterval = 0.5f;

    [Header("Weapons")]
    public GameObject missilePrefab;
    public Transform missileLaunchPoint;
    public int maxMissiles = 10;
    public float missileFireRate = 1.5f;

    [Header("Combat Distance")]
    public float desiredDistance = 5f;
    public float distanceTolerance = 0.5f;

    private int missilesRemaining;
    private float fireTimer;

    private Rigidbody2D rb;
    private Transform owner;
    private EnemyHealth currentTarget;

    private bool returning;
    private float searchTimer;

    public void Initialize(Transform playerShip)
    {
        owner = playerShip;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        missilesRemaining = maxMissiles;
    }

    void Update()
    {
        if (returning)
            return;

        searchTimer -= Time.deltaTime;

        if (searchTimer <= 0f)
        {
            FindNearestEnemy();
            searchTimer = targetSearchInterval;
        }

        if (currentTarget == null)
            FindNearestEnemy();

        if (currentTarget != null && missilesRemaining > 0)
        {
            float distance = Vector2.Distance(
                transform.position,
                currentTarget.transform.position
            );

            bool inFiringRange =
                Mathf.Abs(distance - desiredDistance) <= distanceTolerance;

            if (inFiringRange)
            {
                fireTimer -= Time.deltaTime;

                if (fireTimer <= 0f)
                {
                    Debug.Log("HELICOPTER FIRING!");

                    FireMissile();
                    fireTimer = missileFireRate;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (returning)
        {
            ReturnToOwner();
            return;
        }

        if (currentTarget != null)
        {
            MaintainDistance(currentTarget.transform.position);
        }
    }

    void MaintainDistance(Vector2 destination)
    {
        Vector2 direction = destination - rb.position;
        float distance = direction.magnitude;

        if (distance < 0.01f)
            return;

        direction.Normalize();

        // Face the enemy.
        float targetAngle =
            Mathf.Atan2(direction.y, direction.x) *
            Mathf.Rad2Deg - 90f;

        float newAngle = Mathf.MoveTowardsAngle(
            rb.rotation,
            targetAngle,
            rotationSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(newAngle);

        // Calculate the preferred position away from the enemy.
        Vector2 desiredPosition =
            destination - direction * desiredDistance;

        // Move closer if too far away.
        if (distance > desiredDistance + distanceTolerance)
        {
            rb.MovePosition(Vector2.MoveTowards(
                rb.position,
                desiredPosition,
                moveSpeed * Time.fixedDeltaTime
            ));
        }
        // Move away if too close.
        else if (distance < desiredDistance - distanceTolerance)
        {
            rb.MovePosition(Vector2.MoveTowards(
                rb.position,
                desiredPosition,
                moveSpeed * Time.fixedDeltaTime
            ));
        }
    }

    void FindNearestEnemy()
    {
        EnemyHealth[] enemies =
            FindObjectsByType<EnemyHealth>(
                FindObjectsSortMode.None
            );

        EnemyHealth nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy == null)
                continue;

            float distance = Vector2.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        currentTarget = nearestEnemy;
    }

    void FireMissile()
    {
        if (missilePrefab == null || missileLaunchPoint == null)
        {
            Debug.LogWarning(
                "Helicopter missile prefab or launch point is missing!"
            );
            return;
        }

        if (currentTarget == null)
            return;

        GameObject missile = Instantiate(missilePrefab, missileLaunchPoint.position, missileLaunchPoint.rotation);

        Debug.Log("Helicopter missile launched from: " + missileLaunchPoint.position);

        HelicopterMissile projectile =
            missile.GetComponent<HelicopterMissile>();

        if (projectile != null)
        {
            projectile.SetTarget(currentTarget.transform);
            missilesRemaining--;

            Debug.Log(
                "Helicopter missiles remaining: " +
                missilesRemaining
            );

            if (missilesRemaining <= 0)
            {
                BeginReturn();
            }
        }
        else
        {
            Debug.LogError(
                "Helicopter missile prefab is missing HelicopterMissile!"
            );

            Destroy(missile);
        }
    }

    void MoveToward(Vector2 destination)
    {
        Vector2 direction = destination - rb.position;

        if (direction.sqrMagnitude < 0.01f)
            return;

        direction.Normalize();

        Vector2 newPosition = Vector2.MoveTowards(
            rb.position,
            destination,
            moveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);

        float targetAngle =
            Mathf.Atan2(direction.y, direction.x) *
            Mathf.Rad2Deg - 90f;

        float newAngle = Mathf.MoveTowardsAngle(
            rb.rotation,
            targetAngle,
            rotationSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(newAngle);
    }

    public void BeginReturn()
    {
        returning = true;
        currentTarget = null;
    }

    void ReturnToOwner()
    {
        if (owner == null)
        {
            Destroy(gameObject);
            return;
        }

        float distance = Vector2.Distance(
            rb.position,
            owner.position
        );

        if (distance < 0.5f)
        {
            Destroy(gameObject);
            return;
        }

        MoveToward(owner.position);
    }
}
