using UnityEngine;

public class ShipDamageFireEffects : MonoBehaviour
{
    [Header("Fire Prefab")]
    public GameObject firePrefab;

    [Header("Fire Spawn Points")]
    public Transform frontFirePoint;
    public Transform backFirePoint;

    private bool frontFireSpawned = false;
    private bool backFireSpawned = false;

    public void UpdateFireEffects(float currentHealth, float maxHealth)
    {
        if (maxHealth <= 0f)
            return;

        float damagePercentage =
            1f - (currentHealth / maxHealth);

        // Spawn the first fire after losing one-third
        // of the ship's maximum health.
        if (damagePercentage >= 1f / 3f &&
            !frontFireSpawned)
        {
            SpawnFire(frontFirePoint);

            if (frontFirePoint != null && firePrefab != null)
                frontFireSpawned = true;
        }

        // Spawn the second fire after losing two-thirds
        // of the ship's maximum health.
        if (damagePercentage >= 2f / 3f &&
            !backFireSpawned)
        {
            SpawnFire(backFirePoint);

            if (backFirePoint != null && firePrefab != null)
                backFireSpawned = true;
        }
    }

    private void SpawnFire(Transform spawnPoint)
    {
        if (firePrefab == null)
        {
            Debug.LogError("Fire prefab is missing!", this);
            return;
        }

        if (spawnPoint == null)
        {
            Debug.LogError("Fire spawn point is missing!", this);
            return;
        }

        Instantiate(
            firePrefab,
            spawnPoint.position,
            spawnPoint.rotation,
            spawnPoint
        );
    }
}
