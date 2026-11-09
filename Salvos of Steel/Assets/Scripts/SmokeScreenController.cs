using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PowerupInventory))]
public class SmokeScreenController : MonoBehaviour
{
    [Header("Smoke Screen Settings")]
    public GameObject smokeScreenPrefab;

    private PowerupInventory inventory;

    void Awake()
    {
        inventory = GetComponent<PowerupInventory>();
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        // Press 3 to deploy a smoke screen
        if (Keyboard.current.digit3Key.wasPressedThisFrame ||
            Keyboard.current.numpad3Key.wasPressedThisFrame)
        {
            DeploySmokeScreen();
        }
    }

    void DeploySmokeScreen()
    {
        // Make sure the prefab is assigned
        if (smokeScreenPrefab == null)
        {
            Debug.LogError("Smoke Screen Prefab is missing!");
            return;
        }

        // Check inventory and consume one smoke screen
        if (!inventory.UseSmokeScreen())
        {
            Debug.Log("You have no Smoke Screens available!");
            return;
        }

        // Spawn smoke at the player's current position
        Instantiate(
            smokeScreenPrefab,
            transform.position,
            Quaternion.identity
        );

        Debug.Log("SMOKE SCREEN DEPLOYED!");
    }
}
