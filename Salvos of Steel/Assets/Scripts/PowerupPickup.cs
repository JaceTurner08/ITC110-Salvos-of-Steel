
using UnityEngine;

public class PowerupPickup : MonoBehaviour
{
    public enum PowerupType
    {
        EmergencyFuel,
        Airstrike
    }

    [Header("Powerup Settings")]
    public PowerupType powerupType;

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
            return;

        PowerupInventory inventory =
            other.GetComponentInParent<PowerupInventory>();

        if (inventory == null)
            return;

        collected = true;

        switch (powerupType)
        {
            case PowerupType.EmergencyFuel:
                inventory.AddEmergencyFuel();
                Debug.Log("Emergency Fuel collected!");
                break;

            case PowerupType.Airstrike:
                inventory.AddAirstrike();
                Debug.Log("Airstrike collected!");
                break;
        }

        Destroy(gameObject);
    }
}
