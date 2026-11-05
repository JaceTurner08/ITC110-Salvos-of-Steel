using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image healthBarFill;

    void Start()
    {
        UpdateHealthBar();
    }

    void Update()
    {
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (playerHealth == null || healthBarFill == null)
            return;

        float healthPercentage =
            (float)playerHealth.currentHealth / playerHealth.maxHealth;

        healthBarFill.fillAmount = healthPercentage;
    }
}
