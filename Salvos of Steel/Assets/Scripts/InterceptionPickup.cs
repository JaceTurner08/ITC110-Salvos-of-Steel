using UnityEngine;

public class InterceptionPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        PowerupInventory inventory =
            other.GetComponentInParent<PowerupInventory>();

        if (inventory == null)
            return;

        inventory.AddInterception();

        Debug.Log("INTERCEPTION POWERUP COLLECTED!");

        Destroy(gameObject);
    }
}
