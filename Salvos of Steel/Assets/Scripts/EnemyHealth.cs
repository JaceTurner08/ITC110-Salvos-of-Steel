using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Health")]
    public float maxHealth = 80000f;

    private float currentHealth;

    public float CurrentHealth => currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        currentHealth = Mathf.Max(currentHealth, 0f);

        Debug.Log(gameObject.name + " Health: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
