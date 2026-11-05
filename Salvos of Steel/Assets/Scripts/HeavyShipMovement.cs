using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
[RequireComponent(typeof(Rigidbody2D))]
public class HeavyShipMovement : MonoBehaviour
{
    [Header("Movement")]
    // Public variables can be seen and edited in the inspector
    public float acceleration = 2f;
    public float maxForwardSpeed = 8f;
    public float maxReverseSpeed = 3f;
    public float naturalDeceleration = 0.5f;

    [Header("Turning")]
    public float turnSpeed = 45f;

    [Header("Island Collision")]
    [Tooltip("Maximum forward speed while pressing into an obstacle.")]
    public float collisionSpeed = 0.15f;
    // Private variables can't be seen in the inspector
    private Rigidbody2D rb;
    private float currentSpeed = 0f;
    private bool touchingObstacle;
    private Vector2 obstacleNormal;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //read keyboard input using the new input system.
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

        //W accelerates forward; S accelerates backward.
        if (forwardPressed && !reversePressed)
        {
            currentSpeed += acceleration * Time.fixedDeltaTime;
        }
        else if (reversePressed && !forwardPressed)
        {
            currentSpeed -= acceleration * Time.fixedDeltaTime;
        }
        else
        {

                //Gradually slow down when neither key is held,
                 // or when both are held.
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, naturalDeceleration * Time.fixedDeltaTime);
        }

        //Clamp speed to forward and reverse limits.
        currentSpeed = Mathf.Clamp (currentSpeed, -maxReverseSpeed, maxForwardSpeed);

        //If pushing into an obstacle, nearly stop forward movement.
        //Reverse remains available to back away.
        if (touchingObstacle && currentSpeed > 0f)
        {
            Vector2 forward = transform.up;

            //Only slow down if facing into an obstacle.
            if (Vector2.Dot(forward, obstacleNormal) < -0.01f)
            {
                currentSpeed = Mathf.Min(currentSpeed, collisionSpeed);
            }
        }

        //Turn gradually while A or D is held.
        float turnAmount = -turnInput * turnSpeed * Time.fixedDeltaTime;

        rb.MoveRotation(rb.rotation + turnAmount);

        //Move in the direction the ship faces.
        Vector2 movement = currentSpeed * Time.fixedDeltaTime * (Vector2)transform.up;

        rb.MovePosition(rb.position + movement);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        touchingObstacle = false;

        //Find the contact surface that blocks forward movement.
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
        //Recheck other contacts next physicsstep.
        touchingObstacle = false;
    }
}
