
using UnityEngine;

public class SmokeScreenPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PowerupInventory inventory =
            other.GetComponentInParent<PowerupInventory>();

        if (inventory != null)
        {
            inventory.AddSmokeScreen();

            Debug.Log("Smoke Screen collected!");

            Destroy(gameObject);
        }
    }
}
