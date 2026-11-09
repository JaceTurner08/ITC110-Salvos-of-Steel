
using UnityEngine;
using System.Collections;

public class DummyMissileShooter : MonoBehaviour
{
    [Header("Missile Prefabs")]
    public GameObject bigMissilePrefab;
    public GameObject smallMissilePrefab;

    [Header("Launch Settings")]
    public Transform missileLaunchPoint;
    public Transform playerTarget;

    [Header("Missile Settings")]
    public float bigMissileDamage = 6000f;
    public float smallMissileDamage = 4500f;

    public int bigMissileCount = 4;
    public int smallMissileCount = 4;
    public float firingInterval = 0.5f;

    private bool hasFired = false;

    void Start()
    {
        if (playerTarget == null ||
            missileLaunchPoint == null ||
            bigMissilePrefab == null ||
            smallMissilePrefab == null)
        {
            Debug.LogError("Dummy Missile Shooter is missing an assignment!", this);
            return;
        }

        StartCoroutine(FireMissileSequence());
    }

    IEnumerator FireMissileSequence()
    {
        if (hasFired) yield break;

        hasFired = true;

        int bigFired = 0;
        int smallFired = 0;

        int totalMissiles = bigMissileCount + smallMissileCount;

        for (int i = 0; i < totalMissiles; i++)
        {
            bool fireBig =
                bigFired < bigMissileCount &&
                (smallFired >= smallMissileCount || i % 2 == 0);

            GameObject prefab = fireBig
                ? bigMissilePrefab
                : smallMissilePrefab;

            GameObject missile = Instantiate(
                prefab,
                missileLaunchPoint.position,
                missileLaunchPoint.rotation
            );

            if (fireBig)
            {
                MissileProjectile script =
                    missile.GetComponentInChildren<MissileProjectile>();

                if (script != null)
                {
                    script.SetTarget(
                        playerTarget,
                        bigMissileDamage,
                        false,
                        true
                    );

                    bigFired++;
                }
                else
                {
                    Debug.LogError("Big missile prefab has no MissileProjectile script!");
                    Destroy(missile);
                }
            }
            else
            {
                smallMissile script =
                    missile.GetComponentInChildren<smallMissile>();

                if (script != null)
                {
                    script.SetTarget(
                        playerTarget,
                        smallMissileDamage,
                        true
                    );

                    smallFired++;
                }
                else
                {
                    Debug.LogError("Small missile prefab has no smallMissile script!");
                    Destroy(missile);
                }
            }

            // Prevent missiles from hitting their launching ship.
            Collider2D[] missileColliders =
                missile.GetComponentsInChildren<Collider2D>();

            Collider2D[] shooterColliders =
                GetComponentsInChildren<Collider2D>();

            foreach (Collider2D missileCollider in missileColliders)
            {
                foreach (Collider2D shooterCollider in shooterColliders)
                {
                    Physics2D.IgnoreCollision(
                        missileCollider,
                        shooterCollider
                    );
                }
            }

            Debug.Log("Dummy fired: " + prefab.name);

            yield return new WaitForSeconds(firingInterval);
        }

        Debug.Log("DUMMY HAS FIRED ALL MISSILES!");
    }
}
