using UnityEngine;
using System.Collections.Generic;

public class AirstrikeExplosion : MonoBehaviour
{
    [Header("Explosion Damage")]
    public float damage = 20000f;
    public float explosionRadius = 3f;

    private bool hasExploded = false;

    public void ActivateExplosion()
    {
        if (hasExploded)
            return;

        hasExploded = true;
        ApplyExplosionDamage();
    }

    void ApplyExplosionDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            explosionRadius
        );

        HashSet<EnemyHealth> hitEnemies =
            new HashSet<EnemyHealth>();

        HashSet<PlayerHealth> hitPlayers =
            new HashSet<PlayerHealth>();

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy =
                hit.GetComponentInParent<EnemyHealth>();

            PlayerHealth player =
                hit.GetComponentInParent<PlayerHealth>();

            if (enemy != null && hitEnemies.Add(enemy))
            {
                enemy.TakeDamage(damage);
                Debug.Log("Enemy hit by airstrike!");
            }

            if (player != null && hitPlayers.Add(player))
            {
                player.TakeDamage(Mathf.RoundToInt(damage));
                Debug.Log("Player hit by airstrike!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}
