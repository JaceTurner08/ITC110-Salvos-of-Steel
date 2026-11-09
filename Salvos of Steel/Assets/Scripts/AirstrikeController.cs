using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class AirstrikeController : MonoBehaviour
{
    [Header("Airstrike Prefabs")]
    public GameObject targetingCirclePrefab;
    public GameObject airplanePrefab;
    public GameObject explosionPrefab;

    [Header("Airstrike Timing")]
    public float planeArrivalDelay = 6f;
    public float planeFlightDuration = 1.5f;
    public float explosionDelay = 0.5f;
    public float explosionAnimationDuration = 2f;

    private PowerupInventory inventory;

    private GameObject targetingCircle;

    private bool targetingActive = false;
    private bool airstrikeInProgress = false;

    private Vector3 selectedStrikePosition;

    void Awake()
    {
        inventory = GetComponent<PowerupInventory>();
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null)
            return;

        // Press 2 to activate or cancel targeting.
        if (Keyboard.current.digit2Key.wasPressedThisFrame ||
            Keyboard.current.numpad2Key.wasPressedThisFrame)
        {
            if (targetingActive)
            {
                CancelAirstrike();
            }
            else if (!airstrikeInProgress)
            {
                ActivateAirstrike();
            }
        }

        if (targetingActive)
        {
            UpdateTargetingCircle();

            // Left-click confirms the target location.
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                ConfirmAirstrike();
            }
        }
    }

    void ActivateAirstrike()
    {
        if (inventory == null || inventory.airstrikeCount <= 0)
        {
            Debug.Log("No Airstrikes available!");
            return;
        }

        if (targetingCirclePrefab == null ||
            airplanePrefab == null ||
            explosionPrefab == null)
        {
            Debug.LogError(
                "Assign the targeting circle, airplane, and explosion prefabs!"
            );

            return;
        }

        if (!inventory.UseAirstrike())
            return;

        targetingActive = true;

        targetingCircle = Instantiate(targetingCirclePrefab);

        Debug.Log("AIRSTRIKE TARGETING ACTIVATED!");
    }

    void CancelAirstrike()
    {
        targetingActive = false;

        if (targetingCircle != null)
        {
            Destroy(targetingCircle);
        }

        // Return the unused Airstrike to inventory.
        inventory.AddAirstrike();

        Debug.Log("AIRSTRIKE CANCELLED!");
    }

    void UpdateTargetingCircle()
    {
        if (Camera.main == null || targetingCircle == null)
            return;

        Vector3 mousePosition =
            Mouse.current.position.ReadValue();

        Vector3 worldPosition =
            Camera.main.ScreenToWorldPoint(mousePosition);

        worldPosition.z = 0f;

        targetingCircle.transform.position = worldPosition;
    }

    void ConfirmAirstrike()
    {
        selectedStrikePosition = targetingCircle.transform.position;

        targetingActive = false;

        Destroy(targetingCircle);

        Debug.Log("AIRSTRIKE LOCATION CONFIRMED!");

        airstrikeInProgress = true;

        StartCoroutine(PerformAirstrike());
    }

    IEnumerator PerformAirstrike()
    {
        // Wait six seconds before the airplane arrives.
        yield return new WaitForSeconds(planeArrivalDelay);

        Debug.Log("AIRPLANE INCOMING!");

        // Fly the airplane across the screen.
        yield return StartCoroutine(FlyAirplane());

        Debug.Log("AIRPLANE HAS LEFT THE SCREEN!");

        // Wait half a second after the airplane exits.
        yield return new WaitForSeconds(explosionDelay);

        // Spawn the explosion at the selected location.
        GameObject explosion = Instantiate(
            explosionPrefab,
            selectedStrikePosition,
            Quaternion.identity
        );

        AirstrikeExplosion explosionScript = explosion.GetComponent<AirstrikeExplosion>();

        if (explosionScript != null)
        {
            explosionScript.ActivateExplosion();
        }
        else
        {
            Debug.LogError("AirstrikeExplosion component is missing!");
        }

        Debug.Log("AIRSTRIKE EXPLOSION!");

        // Remove the explosion after its animation finishes.
        Destroy(explosion, explosionAnimationDuration);

        airstrikeInProgress = false;
    }

    IEnumerator FlyAirplane()
    {
        Camera cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("Main Camera not found!");
            airstrikeInProgress = false;
            yield break;
        }

        // Find the selected location's vertical screen position.
        float viewportY =
            cam.WorldToViewportPoint(selectedStrikePosition).y;

        // Position the airplane outside both screen edges.
        float cameraDistance = Mathf.Abs(cam.transform.position.z);

        Vector3 startPosition = cam.ViewportToWorldPoint(
            new Vector3(1.1f, viewportY, cameraDistance)
        );

        Vector3 endPosition = cam.ViewportToWorldPoint(
            new Vector3(-0.1f, viewportY, cameraDistance)
        );

        startPosition.z = 0f;
        endPosition.z = 0f;

        // Spawn airplane on the right.
        GameObject airplane = Instantiate(
            airplanePrefab,
            startPosition,
            Quaternion.identity
        );

        float elapsed = 0f;

        float flightDuration = Mathf.Max(0.1f, planeFlightDuration);

        // Move airplane from right to left.
        while (elapsed < flightDuration)
        {
            float progress = elapsed / flightDuration;

            airplane.transform.position = Vector3.Lerp(
                startPosition,
                endPosition,
                progress
            );

            elapsed += Time.deltaTime;

            yield return null;
        }

        airplane.transform.position = endPosition;

        Destroy(airplane);
    }
}
