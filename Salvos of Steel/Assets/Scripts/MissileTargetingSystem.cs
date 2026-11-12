using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MissileTargetingSystem : MonoBehaviour
{
    [Header("Missile Prefabs")]
    public GameObject bigMissilePrefab;
    public GameObject smallMissilePrefab;

    [Header("Launch Settings")]
    public Transform missileLaunchPoint;

    [Header("Targeting Settings")]
    public float lockOnTime = 5f;
    public LayerMask enemyLayer;

    [Header("Big Missile Settings")]
    public int maxBigMissiles = 4;
    public int maxBigMissileAmmo = 4;
    public float bigMissileDamage = 6000f;
    public float bigMissileReloadTime = 10f;

    [Header("Small Missile Settings")]
    public int maxSmallMissiles = 4;
    public int maxSmallMissileAmmo = 4;
    public float smallMissileDamage = 2500f;
    public float smallMissileReloadTime = 5f;

    [Header("Targeting UI")]
    public Vector2 uiPosition = new Vector2(20f, 100f);

    private int bigMissilesRemaining;
    private int smallMissilesRemaining;

    private bool bigMissileReloading;
    private bool smallMissileReloading;

    private float bigReloadTimer;
    private float smallReloadTimer;

    private Transform currentTarget;
    private float currentLockTime;
    private bool targetLocked;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        bigMissilesRemaining = maxBigMissileAmmo;
        smallMissilesRemaining = maxSmallMissileAmmo;
    }

    void Update()
    {
        UpdateTargeting();

        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (targetLocked)
                FireBigMissile();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (targetLocked)
                FireSmallMissile();
        }
    }

    void UpdateTargeting()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null || Mouse.current == null)
        {
            ResetTargeting();
            return;
        }

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(mousePosition);

        Collider2D hit = Physics2D.OverlapPoint(
            new Vector2(worldPosition.x, worldPosition.y),
            enemyLayer
        );

        Transform newTarget = null;

        if (hit != null)
        {
            EnemyHealth enemy =
                hit.GetComponentInParent<EnemyHealth>();

            HelicopterHealth helicopter =
                hit.GetComponentInParent<HelicopterHealth>();

            if (enemy != null)
                newTarget = enemy.transform;
            else if (helicopter != null)
                newTarget = helicopter.transform;
        }

        if (newTarget == null)
        {
            ResetTargeting();
            return;
        }

        // Prevent locking when both ships are inside the same smoke circle.
        if (SmokeScreenArea.AreBothInsideSameSmoke(transform, newTarget))
        {
            ResetTargeting();
            return;
        }

        // Reset the lock timer when changing targets.
        if (currentTarget != newTarget)
        {
            currentTarget = newTarget;
            currentLockTime = 0f;
            targetLocked = false;
        }

        if (!targetLocked)
        {
            currentLockTime += Time.deltaTime;

            if (currentLockTime >= lockOnTime)
            {
                targetLocked = true;
                Debug.Log("TARGET LOCKED: " + currentTarget.name);
            }
        }
    }

    void ResetTargeting()
    {
        currentTarget = null;
        currentLockTime = 0f;
        targetLocked = false;
    }

    void FireBigMissile()
    {
        if (bigMissilesRemaining <= 0)
        {
            Debug.Log("NO BIG MISSILES AVAILABLE!");
            return;
        }

        if (bigMissilePrefab == null ||
            missileLaunchPoint == null)
        {
            Debug.LogError("Big missile prefab or launch point is missing!");
            return;
        }

        FireMissile(
            bigMissilePrefab,
            bigMissileDamage,
            true
        );

        bigMissilesRemaining--;

        if (!bigMissileReloading &&
            bigMissilesRemaining < maxBigMissileAmmo)
        {
            StartCoroutine(ReloadBigMissiles());
        }
    }

    void FireSmallMissile()
    {
        if (smallMissilesRemaining <= 0)
        {
            Debug.Log("NO SMALL MISSILES AVAILABLE!");
            return;
        }

        if (smallMissilePrefab == null ||
            missileLaunchPoint == null)
        {
            Debug.LogError("Small missile prefab or launch point is missing!");
            return;
        }

        FireMissile(
            smallMissilePrefab,
            smallMissileDamage,
            true
        );

        smallMissilesRemaining--;

        if (!smallMissileReloading &&
            smallMissilesRemaining < maxSmallMissileAmmo)
        {
            StartCoroutine(ReloadSmallMissiles());
        }
    }

    void FireMissile(
        GameObject missilePrefab,
        float damage,
        bool allowHelicopterDamage = false)
    {
        if (currentTarget == null)
            return;

        GameObject missile = Instantiate(
            missilePrefab,
            missileLaunchPoint.position,
            missileLaunchPoint.rotation
        );

        MissileProjectile projectile =
            missile.GetComponentInChildren<MissileProjectile>();

        if (projectile == null)
        {
            Debug.LogError(
                "Missile prefab is missing MissileProjectile!",
                missile
            );

            Destroy(missile);
            return;
        }

        projectile.SetTarget(
            currentTarget,
            damage,
            allowHelicopterDamage,
            false
        );

        Debug.Log(
            "Fired " + missilePrefab.name +
            " at " + currentTarget.name
        );
    }

    IEnumerator ReloadBigMissiles()
    {
        bigMissileReloading = true;
        bigReloadTimer = bigMissileReloadTime;

        yield return new WaitForSeconds(bigMissileReloadTime);

        // Refill all big missile ammunition at once.
        bigMissilesRemaining = maxBigMissileAmmo;

        bigReloadTimer = 0f;
        bigMissileReloading = false;

        Debug.Log("BIG MISSILES FULLY RELOADED!");
    }

    IEnumerator ReloadSmallMissiles()
    {
        smallMissileReloading = true;
        smallReloadTimer = smallMissileReloadTime;

        yield return new WaitForSeconds(smallMissileReloadTime);

        // Refill all small missile ammunition at once.
        smallMissilesRemaining = maxSmallMissileAmmo;

        smallReloadTimer = 0f;
        smallMissileReloading = false;

        Debug.Log("SMALL MISSILES FULLY RELOADED!");
    }

    void OnGUI()
    {
        GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
        titleStyle.fontSize = 17;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.normal.textColor = Color.white;

        GUIStyle normalStyle = new GUIStyle(GUI.skin.label);
        normalStyle.fontSize = 14;
        normalStyle.normal.textColor = Color.white;

        float x = uiPosition.x;
        float y = uiPosition.y;

        GUI.Label(
            new Rect(x, y, 250, 25),
            "MISSILE SYSTEM",
            titleStyle
        );

        y += 25;

        GUI.Label(
            new Rect(x, y, 250, 22),
            "BIG MISSILES: " +
            bigMissilesRemaining + "/" +
            maxBigMissileAmmo,
            normalStyle
        );

        y += 22;

        if (bigMissileReloading)
        {
            GUI.Label(
                new Rect(x, y, 250, 22),
                "Big Reload: " +
                Mathf.Max(0f, bigReloadTimer).ToString("F1") + "s",
                normalStyle
            );

            y += 22;
        }

        GUI.Label(
            new Rect(x, y, 250, 22),
            "SMALL MISSILES: " +
            smallMissilesRemaining + "/" +
            maxSmallMissileAmmo,
            normalStyle
        );

        y += 22;

        if (smallMissileReloading)
        {
            GUI.Label(
                new Rect(x, y, 250, 22),
                "Small Reload: " +
                Mathf.Max(0f, smallReloadTimer).ToString("F1") + "s",
                normalStyle
            );

            y += 22;
        }

        y += 8;

        if (currentTarget != null)
        {
            if (targetLocked)
            {
                GUI.Label(
                    new Rect(x, y, 300, 25),
                    "TARGET LOCKED: " + currentTarget.name,
                    titleStyle
                );
            }
            else
            {
                GUI.Label(
                    new Rect(x, y, 300, 25),
                    "LOCKING: " +
                    currentLockTime.ToString("F1") +
                    " / " + lockOnTime + "s",
                    normalStyle
                );
            }
        }
        else
        {
            GUI.Label(
                new Rect(x, y, 300, 25),
                "NO TARGET",
                normalStyle
            );
        }
    }
}
