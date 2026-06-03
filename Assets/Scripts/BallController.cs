using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    [Header("Speed Settings")]
    public float defaultSpeed = 8f;
    [Tooltip("How much the base speed increases every time a player is hit.")]
    public float speedIncreasePerHit = 0.5f;

    private float currentBaseSpeed;
    private float currentActiveSpeed; // Includes multipliers
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Force the physics settings in code to ensure zero gravity and zero drag
        rb.gravityScale = 0f;
        rb.linearDamping = 0f; // Unity 6 uses linearDamping instead of linearDrag
        rb.angularDamping = 0f;

        ResetBall();
    }

    public void ResetBall()
    {
        currentBaseSpeed = defaultSpeed;
        currentActiveSpeed = defaultSpeed;

        transform.position = Vector3.zero;

        // Launch in a random direction
        float xDir = Random.value < 0.5f ? -1f : 1f;
        float yDir = Random.Range(-0.5f, 0.5f);
        Vector2 launchDirection = new Vector2(xDir, yDir).normalized;

        rb.linearVelocity = launchDirection * currentActiveSpeed;
    }

    void FixedUpdate()
    {
        // 1. THE CRUISE CONTROL:
        // No matter what Unity's physics engine tries to do, we force the ball 
        // to maintain exactly the active speed every single physics frame.
        rb.linearVelocity = rb.linearVelocity.normalized * currentActiveSpeed;

        // 2. THE ANTI-STUCK FIX:
        PreventStuckBall();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Bat"))
        {
            currentBaseSpeed += speedIncreasePerHit;
            float multiplier = 1f;

            PongDroneController player = collision.gameObject.GetComponentInParent<PongDroneController>();
            if (player != null && collision.gameObject.CompareTag("Bat"))
            {
                multiplier = player.GetBatMultiplier();
            }

            currentActiveSpeed = currentBaseSpeed * multiplier;

            ContactPoint2D contact = collision.GetContact(0);
            Vector2 paddleCenter = collision.transform.position;
            Vector2 bounceDirection = (contact.point - paddleCenter).normalized;

            rb.linearVelocity = bounceDirection * currentActiveSpeed;
        }

        // Notice we removed the "else" statement for the walls! 
        // Because of our new FixedUpdate, Unity handles the wall bounce angle 
        // automatically, and FixedUpdate ensures the speed stays perfect.
    }

    private void PreventStuckBall()
    {
        // A classic Pong problem is the ball bouncing purely vertically or horizontally forever.
        // This adds a microscopic tilt to the velocity if it's too perfectly straight.
        Vector2 vel = rb.linearVelocity;

        if (Mathf.Abs(vel.x) < 0.1f) vel.x = Mathf.Sign(vel.x) * 0.5f;
        if (Mathf.Abs(vel.y) < 0.1f) vel.y = Mathf.Sign(vel.y) * 0.5f;

        // Re-apply if nudged (FixedUpdate will re-normalize the speed on the next line anyway)
        rb.linearVelocity = vel;
    }
}