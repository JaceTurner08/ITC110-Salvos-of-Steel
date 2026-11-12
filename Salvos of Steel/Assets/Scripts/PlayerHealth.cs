using UnityEngine;

[RequireComponent(typeof(ShipDamageFireEffects))]
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
    private ShipDamageFireEffects fireEffects;

    void Awake()
    {
        fireEffects = GetComponent<ShipDamageFireEffects>();
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDestroyed)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("Player Health: " + currentHealth + " / " + maxHealth);

        fireEffects.UpdateFireEffects(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            isDestroyed = true;
            Debug.Log("PLAYER SHIP DESTROYED!");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((damagingLayers.value & (1 << collision.gameObject.layer)) != 0)
        {
            TakeDamage(collisionDamage);
        }
    }
}
