using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PowerupInventory))]
public class InterceptionSystem : MonoBehaviour
{
    [Header("Interception Settings")]
    public float activationWindow = 2f;
    public float cooldownDuration = 15f;

    [Header("Target Settings")]
    [Tooltip("The Transform that enemy missiles are targeting.")]
    public Transform playerTarget;

    private Camera mainCamera;

    private PowerupInventory inventory;

    private bool interceptionActive = false;

    private float activationTimer = 0f;
    private float cooldownTimer = 0f;

    void Awake()
    {
        inventory = GetComponent<PowerupInventory>();
    }

    void Start()
    {
        mainCamera = Camera.main;

        if (playerTarget == null)
        {
            playerTarget = transform;
        }
    }

    void Update()
    {
        if (Keyboard.current == null || Mouse.current == null)
            return;

        // Update cooldown.
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Max(0f, cooldownTimer);
        }

        // Update the two-second interception window.
        if (interceptionActive)
        {
            activationTimer -= Time.deltaTime;

            if (activationTimer <= 0f)
            {
                interceptionActive = false;
                activationTimer = 0f;

                Debug.Log("INTERCEPTION WINDOW CLOSED!");
            }
        }

        // Press 4 to activate.
        if (Keyboard.current.digit4Key.wasPressedThisFrame ||
            Keyboard.current.numpad4Key.wasPressedThisFrame)
        {
            ActivateInterception();
        }

        // Click an incoming missile during the active window.
        if (interceptionActive &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryInterceptMissile();
        }
    }

    void ActivateInterception()
    {
        if (interceptionActive)
        {
            Debug.Log("INTERCEPTION SYSTEM IS ALREADY ACTIVE!");
            return;
        }

        if (cooldownTimer > 0f)
        {
            Debug.Log(
                "INTERCEPTION SYSTEM RECHARGING: " +
                cooldownTimer.ToString("F1") + "s"
            );

            return;
        }

        // Check and consume an Interception charge.
        if (!inventory.UseInterception())
        {
            Debug.Log("NO INTERCEPTION POWERUPS AVAILABLE!");
            return;
        }

        interceptionActive = true;
        activationTimer = activationWindow;

        cooldownTimer = cooldownDuration;

        Debug.Log("INTERCEPTION ACTIVATED! 2 SECONDS!");
    }


    void TryInterceptMissile()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(mousePosition);

        Collider2D[] hits = Physics2D.OverlapPointAll(
            new Vector2(worldPosition.x, worldPosition.y)
        );

        int interceptedCount = 0;

        foreach (Collider2D hit in hits)
        {
            MissileProjectile missile =
                hit.GetComponentInParent<MissileProjectile>();

            if (missile == null)
                continue;

            // Only intercept missiles targeting the player.
            if (!missile.IsTargeting(playerTarget))
                continue;

            // Permanently disable this missile's homing.
            missile.PermanentlyDisableGuidance();

            interceptedCount++;
        }

        if (interceptedCount > 0)
        {
            Debug.Log(
                "INTERCEPTED " + interceptedCount + " ENEMY MISSILE(S)!"
            );
        }
        else
        {
            Debug.Log("NO INCOMING ENEMY MISSILES SELECTED!");
        }
    }


    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 15;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        string message;

        if (interceptionActive)
        {
            message = "INTERCEPTION ACTIVE: " +
                      activationTimer.ToString("F1") + "s";
        }
        else if (cooldownTimer > 0f)
        {
            message = "INTERCEPTION RECHARGING: " +
                      cooldownTimer.ToString("F1") + "s";
        }
        else
        {
            message = "INTERCEPTION READY [4]";
        }

        GUI.Label(
            new Rect(20f, 250f, 350f, 25f),
            message,
            style
        );
    }
}
