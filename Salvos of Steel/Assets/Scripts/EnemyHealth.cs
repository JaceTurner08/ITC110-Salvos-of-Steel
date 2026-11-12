using UnityEngine;

[RequireComponent(typeof(ShipDamageFireEffects))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    public float maxHealth = 80000f;

    private float currentHealth;

    private ShipDamageFireEffects fireEffects;

    public float CurrentHealth => currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
        fireEffects = GetComponent<ShipDamageFireEffects>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f);

        Debug.Log(gameObject.name + " Health: " + currentHealth);

        fireEffects.UpdateFireEffects(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
