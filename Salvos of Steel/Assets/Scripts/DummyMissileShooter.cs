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

    [Header("Firing Timing")]
    public float firingInterval = 0.5f;
    public float repeatDelay = 5f;

    void Start()
    {
        if (playerTarget == null ||
            missileLaunchPoint == null ||
            bigMissilePrefab == null ||
            smallMissilePrefab == null)
        {
            Debug.LogError(
                "Dummy Missile Shooter is missing an assignment!", this);
            return;
        }

        StartCoroutine(FireMissileSequence());
    }

    IEnumerator FireMissileSequence()
    {
        while (true)
        {
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

                MissileProjectile script =
                    missile.GetComponentInChildren<MissileProjectile>();

                if (script != null)
                {
                    float damage = fireBig
                        ? bigMissileDamage
                        : smallMissileDamage;

                    script.SetTarget(
                        playerTarget,
                        damage,
                        false,
                        true
                    );

                    if (fireBig)
                        bigFired++;
                    else
                        smallFired++;
                }
                else
                {
                    Debug.LogError(
                        "Missile prefab is missing MissileProjectile!",
                        missile
                    );

                    Destroy(missile);
                    yield return new WaitForSeconds(firingInterval);
                    continue;
                }

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

            Debug.Log("DUMMY FINISHED FIRING! Next salvo in " +
                repeatDelay + " seconds.");

            yield return new WaitForSeconds(repeatDelay);
        }
    }
}