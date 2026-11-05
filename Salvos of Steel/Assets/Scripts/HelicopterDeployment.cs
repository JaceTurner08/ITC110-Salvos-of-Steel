using UnityEngine;
using UnityEngine.InputSystem;

public class HelicopterDeployment : MonoBehaviour
{
    [Header("Helicopter Settings")]
    public GameObject helicopterPrefab;
    public Transform helicopterSpawnPoint;

    private GameObject activeHelicopter;

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            DeployHelicopter();
        }
    }

    void DeployHelicopter()
    {
        // Prevent deploying another helicopter while one is active.
        if (activeHelicopter != null)
        {
            Debug.Log("Helicopter is already deployed.");
            return;
        }

        if (helicopterPrefab == null || helicopterSpawnPoint == null)
        {
            Debug.LogWarning("Assign the helicopter prefab and spawn point!");
            return;
        }

        activeHelicopter = Instantiate(
            helicopterPrefab,
            helicopterSpawnPoint.position,
            helicopterSpawnPoint.rotation
        );

        HelicopterController controller =
            activeHelicopter.GetComponent<HelicopterController>();

        if (controller != null)
        {
            controller.Initialize(transform);
        }
        else
        {
            Debug.LogError("Helicopter prefab is missing HelicopterController!");
            Destroy(activeHelicopter);
            activeHelicopter = null;
        }
    }
}
