using UnityEngine;
using UnityEngine.InputSystem;

public class ChaffController : MonoBehaviour
{
    [Header("Chaff Settings")]
    public float guidanceDisruptionDuration = 5f;
    public float cooldownDuration = 12f;

    [Header("Chaff Effect")]
    public GameObject chaffEffectPrefab;
    public float effectDuration = 0.6f;

    private float nextChaffTime = 0f;

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (Time.time >= nextChaffTime)
            {
                DeployChaff();
            }
            else
            {
                float remaining = nextChaffTime - Time.time;

                Debug.Log("Chaff recharging: " +
                    remaining.ToString("F1") + " seconds remaining.");
            }
        }
    }

    void DeployChaff()
    {
        // Start cooldown immediately.
        nextChaffTime = Time.time + cooldownDuration;

        Debug.Log("CHAFF DEPLOYED!");

        // Disrupt incoming big missiles.
        MissileProjectile[] bigMissiles =
            FindObjectsByType<MissileProjectile>(
                FindObjectsSortMode.None);

        foreach (MissileProjectile missile in bigMissiles)
        {
            if (missile.IsTargeting(transform))
            {
                missile.DisableGuidance(guidanceDisruptionDuration);
            }
        }

        // Disrupt incoming small missiles.
        smallMissile[] smallMissiles =
            FindObjectsByType<smallMissile>(
                FindObjectsSortMode.None);

        foreach (smallMissile missile in smallMissiles)
        {
            if (missile.IsTargeting(transform))
            {
                missile.DisableGuidance(guidanceDisruptionDuration);
            }
        }

        // Spawn the chaff animation.
        if (chaffEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                chaffEffectPrefab,
                transform.position,
                Quaternion.identity
            );

            Destroy(effect, effectDuration);
        }
        else
        {
            Debug.LogWarning("Chaff Effect Prefab is missing!");
        }
    }
}
