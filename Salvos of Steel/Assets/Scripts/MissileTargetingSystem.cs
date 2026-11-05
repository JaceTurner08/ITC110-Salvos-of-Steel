using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MissileTargetingSystem : MonoBehaviour
{
    [Header("Missile Prefabs")]
    public GameObject bigMissilePrefab;
    public GameObject smallMissilePrefab;

    [Header("Launch Position")]
    public Transform missileLaunchPoint;

    [Header("Targeting")]
    public float lockOnTime = 5f;
    public LayerMask enemyLayer;

    [Header("Big Missiles")]
    public int maxBigMissiles = 4;
    public float bigMissileDamage = 8000f;
    public float bigMissileReload = 10f;

    [Header("Small Missiles")]
    public int maxSmallMissiles = 4;
    public float smallMissileDamage = 4500f;
    public float smallMissileReload = 5f;

    [Header("Ammo Count")]
    public int maxBigMissileAmmo = 4;
    public int maxSmallMissileAmmo = 4;

    public float bigMissileReloadTime = 10f;
    public float smallMissileReloadTime = 5f;

    [Header("Helicopter UI")]
    public float helicopterInitialCooldown = 15f;
    public float helicopterCooldown = 60f;
    private float helicopterTimer;
    private bool helicopterReady = false;
    private bool helicopterDeployed = false;
    private bool bigMissileReloading = false;
    private bool smallMissileReloading = false;

    private int bigMissiles;
    private int smallMissiles;

    private float lockTimer = 0f;
    private EnemyHealth currentTarget;

    private bool isBigReloading = false;
    private bool isSmallReloading = false;

    void Start()
    {
        bigMissiles = maxBigMissiles;
        smallMissiles = maxSmallMissiles;
        helicopterTimer = helicopterInitialCooldown;
    }

    void Update()
    {
        UpdateHelicopterTimer();

        if (Mouse.current == null)
            return;

        // Convert mouse position into world coordinates.
        Vector3 mousePosition = Mouse.current.position.ReadValue();

        Vector3 worldPosition =
            Camera.main.ScreenToWorldPoint(mousePosition);

        Vector2 mouseWorldPosition = new Vector2(
            worldPosition.x,
            worldPosition.y
        );

        // Find an enemy underneath the mouse.
        Collider2D hit = Physics2D.OverlapPoint(
            mouseWorldPosition,
            enemyLayer
        );

        EnemyHealth detectedTarget = null;

        if (hit != null)
        {
            detectedTarget =
                hit.GetComponentInParent<EnemyHealth>();
        }

        // Check whether the target has changed.
        if (detectedTarget != currentTarget)
        {
            currentTarget = detectedTarget;
            lockTimer = 0f;
        }

        // Build the lock-on timer.
        if (currentTarget != null)
        {
            lockTimer += Time.deltaTime;

            lockTimer = Mathf.Min(lockTimer, lockOnTime);
        }
        else
        {
            lockTimer = 0f;
        }

        // Fire big missile with left mouse button.
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (IsTargetLocked() && bigMissiles > 0)
            {
                bigMissiles--;
                
                FireMissile(bigMissilePrefab, bigMissileDamage);

                if (!isBigReloading)
                {
                    StartCoroutine(ReloadBigMissiles());
                }
            }
        }

        // Fire small missile with right mouse button.
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (IsTargetLocked() && smallMissiles > 0)
            {
                smallMissiles--;

                FireMissile(smallMissilePrefab, smallMissileDamage);

                if (!isSmallReloading)
                {
                    StartCoroutine(ReloadSmallMissiles());
                }
            }
        }
    }

    bool IsTargetLocked()
    {
        return currentTarget != null &&
               lockTimer >= lockOnTime;
    }

    void FireMissile(GameObject missilePrefab, float damage)
    {
        if (missilePrefab == null || missileLaunchPoint == null || currentTarget == null)
            return;

        GameObject missile = Instantiate(missilePrefab, missileLaunchPoint.position, missileLaunchPoint.rotation);

        MissileProjectile projectile = missile.GetComponent<MissileProjectile>();

        if (projectile != null)
        {
            projectile.SetTarget(currentTarget.transform, damage);
        }
    }


    private IEnumerator ReloadBigMissiles()
    {
        bigMissileReloading = true;

        // Wait for 10 seconds.
        yield return new WaitForSeconds(bigMissileReloadTime);

        // Restore all big missiles simultaneously.
        bigMissiles = maxBigMissileAmmo;

        bigMissileReloading = false;
    }

    private IEnumerator ReloadSmallMissiles()
    {
        smallMissileReloading = true;

        // Wait for 5 seconds.
        yield return new WaitForSeconds(smallMissileReloadTime);

        // Restore all small missiles simultaneously.
        smallMissiles = maxSmallMissileAmmo;

        smallMissileReloading = false;
    }

    void UpdateHelicopterTimer()
    {
        if (helicopterTimer > 0f)
        {
            helicopterTimer -= Time.deltaTime;
            helicopterTimer = Mathf.Max(0f, helicopterTimer);
        }

        if (helicopterTimer <= 0f && !helicopterDeployed)
        {
            helicopterReady = true;
        }
    }


    // Display ammunition and targeting status.
    void OnGUI()
    {
        GUI.Label(
            new Rect(20, 20, 250, 25),
            "BIG MISSILES: " + bigMissiles + "/" + maxBigMissiles
        );

        GUI.Label(
            new Rect(20, 45, 250, 25),
            "SMALL MISSILES: " + smallMissiles + "/" + maxSmallMissiles
        );

        if (currentTarget != null)
        {
            GUI.Label(
                new Rect(20, 75, 300, 25),
                "TARGET LOCK: " +
                Mathf.Clamp(lockTimer, 0, lockOnTime).ToString("F1")
                + " / " + lockOnTime + " seconds"
            );

            if (IsTargetLocked())
            {
                GUI.Label(
                    new Rect(20, 100, 250, 25),
                    "TARGET LOCKED - FIRE"
                );
            }
        }
        else
        {
            GUI.Label(
                new Rect(20, 75, 250, 25),
                "SEARCHING FOR TARGET..."
            );
        }

        if (helicopterDeployed)
        {
            GUI.Label(
                new Rect(20, 130, 300, 25),
                "HELICOPTER DEPLOYED"
            );
        }
        else if (helicopterReady)
        {
            GUI.Label(
                new Rect(20, 130, 300, 25),
                "HELICOPTER READY - PRESS H"
            );
        }
        else
        {
            GUI.Label(
                new Rect(20, 130, 300, 25),
                "HELICOPTER READY IN: " +
                Mathf.CeilToInt(helicopterTimer) + "s"
            );
        }
    }
}
