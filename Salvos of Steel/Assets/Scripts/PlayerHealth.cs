using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Health")]
    public int maxHealth = 80000;
    public int currentHealth;

    [Header("Collision Damage")]
    public int collisionDamage = 3000;

    [Tooltip("Layers that damage the player, such as Islands and Boundaries.")]
    public LayerMask damagingLayers;

    private bool isDestroyed = false;

    void Start()
    {
        // Give the player full health when the game starts.
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDestroyed)
            return;

        currentHealth -= damage;

        // Prevent health from going below zero.
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("Player Health: " + currentHealth + " / " + maxHealth);

        if (currentHealth <= 0)
        {
            isDestroyed = true;
            Debug.Log("PLAYER SHIP DESTROYED!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check whether the object belongs to a damaging layer.
        if ((damagingLayers.value & (1 << collision.gameObject.layer)) != 0)
        {
            TakeDamage(collisionDamage);
        }
    }
}
