using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class HeavyShipMovement : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration = 2f;
    public float maxForwardSpeed = 8f;
    public float maxReverseSpeed = 3f;
    public float naturalDeceleration = 0.5f;

    [Header("Turning")]
    public float turnSpeed = 45f;

    [Header("Island Collision")]
    public float collisionSpeed = 0.15f;

    [Header("Emergency Fuel")]
    public float fuelBoostMultiplier = 1.35f;
    public float fuelBoostDuration = 10f;

    private Rigidbody2D rb;
    private PowerupInventory inventory;

    private float currentSpeed = 0f;
    private bool touchingObstacle;
    private Vector2 obstacleNormal;

    private bool fuelBoostActive = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        inventory = GetComponent<PowerupInventory>();
    }

    void Update()
    {
        // Press 1 to activate Emergency Fuel.
        if (Keyboard.current != null &&
            (Keyboard.current.digit1Key.wasPressedThisFrame ||
             Keyboard.current.numpad1Key.wasPressedThisFrame))
        {
            ActivateEmergencyFuel();
        }
    }

    void ActivateEmergencyFuel()
    {
        // Don't activate if already boosted.
        if (fuelBoostActive)
        {
            Debug.Log("Emergency Fuel is already active!");
            return;
        }

        // Check whether the player has Emergency Fuel.
        if (inventory == null || !inventory.UseEmergencyFuel())
        {
            Debug.Log("No Emergency Fuel available!");
            return;
        }

        // Start the temporary performance boost.
        StartCoroutine(EmergencyFuelBoost());
    }

    IEnumerator EmergencyFuelBoost()
    {
        fuelBoostActive = true;

        Debug.Log("EMERGENCY FUEL ACTIVATED!");

        yield return new WaitForSeconds(fuelBoostDuration);

        fuelBoostActive = false;

        Debug.Log("Emergency Fuel boost has expired.");
    }

    void FixedUpdate()
    {
        bool forwardPressed = false;
        bool reversePressed = false;
        float turnInput = 0f;

        if (Keyboard.current != null)
        {
            forwardPressed = Keyboard.current.wKey.isPressed;
            reversePressed = Keyboard.current.sKey.isPressed;

            if (Keyboard.current.aKey.isPressed)
            {
                turnInput = -1f;
            }

            if (Keyboard.current.dKey.isPressed)
            {
                turnInput = 1f;
            }
        }

        // Determine movement performance.
        float performanceMultiplier =
            fuelBoostActive ? fuelBoostMultiplier : 1f;

        float boostedAcceleration =
            acceleration * performanceMultiplier;

        float boostedForwardSpeed =
            maxForwardSpeed * performanceMultiplier;

        float boostedReverseSpeed =
            maxReverseSpeed * performanceMultiplier;

        float boostedTurnSpeed =
            turnSpeed * performanceMultiplier;

        // W accelerates forward; S accelerates backward.
        if (forwardPressed && !reversePressed)
        {
            currentSpeed +=
                boostedAcceleration * Time.fixedDeltaTime;
        }
        else if (reversePressed && !forwardPressed)
        {
            currentSpeed -=
                boostedAcceleration * Time.fixedDeltaTime;
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(
                currentSpeed,
                0f,
                naturalDeceleration * Time.fixedDeltaTime
            );
        }

        // Clamp speed to forward and reverse limits.
        currentSpeed = Mathf.Clamp(
            currentSpeed,
            -boostedReverseSpeed,
            boostedForwardSpeed
        );

        // If pushing into an obstacle, nearly stop forward movement.
        if (touchingObstacle && currentSpeed > 0f)
        {
            Vector2 forward = transform.up;

            if (Vector2.Dot(forward, obstacleNormal) < -0.01f)
            {
                currentSpeed = Mathf.Min(
                    currentSpeed,
                    collisionSpeed
                );
            }
        }

        // Turn gradually while A or D is held.
        float turnAmount =
            -turnInput * boostedTurnSpeed * Time.fixedDeltaTime;

        rb.MoveRotation(rb.rotation + turnAmount);

        // Move in the direction the ship faces.
        Vector2 movement =
            currentSpeed * Time.fixedDeltaTime * (Vector2)transform.up;

        rb.MovePosition(rb.position + movement);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        touchingObstacle = false;

        for (int i = 0; i < collision.contactCount; i++)
        {
            ContactPoint2D contact = collision.GetContact(i);

            Vector2 normal = contact.normal;

            if (Vector2.Dot((Vector2)transform.up, normal) < -0.01f)
            {
                touchingObstacle = true;
                obstacleNormal = normal;
                break;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        touchingObstacle = false;
    }
}
