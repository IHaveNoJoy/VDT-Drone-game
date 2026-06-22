using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour
{
    [Header("Speed Settings")]
    public float defaultSpeed = 8f;
    [Tooltip("How much the base speed permanently increases every time a player is hit.")]
    public float speedIncreasePerHit = 0.5f;

    [Header("Charge Settings")]
    [Tooltip("How much the speed temporarily increases per charge level (0.15 = 15%).")]
    public float chargeSpeedBoost = 0.15f;

    private float currentBaseSpeed;
    private float currentActiveSpeed; // Includes temporary multipliers
    private float tempSpeedModifier = 1f; // Resets on every hit
    private Rigidbody2D rb;

    void Start()
    {
        ThemeManager manager = FindObjectOfType<ThemeManager>();
        if (manager != null)
        {
            // Set the sprite to whatever the manager says is the current ball sprite
            GetComponent<SpriteRenderer>().sprite = manager.activeTheme.ballSprite;
        }

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
        tempSpeedModifier = 1f;
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
            // 1. Apply the permanent speed increase to the base speed
            currentBaseSpeed += speedIncreasePerHit;

            // 2. Reset the temporary modifier for this new hit
            tempSpeedModifier = 1f;

            // Grab the controller from either the Player or the Bat (GetComponentInParent handles both!)
            PongDroneController player = collision.gameObject.GetComponentInParent<PongDroneController>();

            // CHANGED: We no longer care if it hit the Bat or the Player specifically.
            // If the player exists and is swinging, give them the charge boost!
            if (player != null)
            {
                int chargeLevel = player.GetChargeLevel();
                tempSpeedModifier += (chargeLevel * chargeSpeedBoost);
                AudioManager.Instance?.PlayHitSound();
            }

            // 3. Calculate the new active speed
            currentActiveSpeed = currentBaseSpeed * tempSpeedModifier;

            // 4. Calculate bounce direction
            ContactPoint2D contact = collision.GetContact(0);
            Vector2 paddleCenter = collision.transform.position;
            Vector2 bounceDirection = (contact.point - paddleCenter).normalized;

            rb.linearVelocity = bounceDirection * currentActiveSpeed;
        }
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