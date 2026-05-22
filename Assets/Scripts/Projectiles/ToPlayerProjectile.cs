using UnityEngine;

// Inherits from Projectile instead of MonoBehaviour
public class ARProjectile : Projectile
{
    [Header("Homing Settings")]
    [Tooltip("How strongly the object homes in. Lower values mean very slight tracking.")]
    public float trackingStrength = 0.8f;

    [Header("AR Hit Detection")]
    public float hitRadius = 0.3f;
    [Tooltip("Select the layers the bullet can crash into (like Walls, Floors, Players).")]
    public LayerMask hitLayers = ~0;

    private Transform playerHeadset;
    private bool hasPassedPlayer = false;
    private bool isFlying = true;

    protected override void Start()
    {
        // 1. Call parent's Start() to trigger the lifeSpan timer
        base.Start();

        // 2. Find the camera and aim instantly
        if (Camera.main != null)
        {
            playerHeadset = Camera.main.transform;
            Vector3 directionToPlayer = playerHeadset.position - transform.position;
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
        }
        else
        {
            Debug.LogError("Main Camera not found! Make sure your headset camera is tagged as 'MainCamera'.");
            isFlying = false;
        }
    }

    protected override void Update()
    {
        // 1. Track parent's arming timer
        timeElapsed += Time.deltaTime;

        if (!isFlying) return;

        // 2. Homing Logic (Dodging Mechanics)
        if (playerHeadset != null && !hasPassedPlayer)
        {
            Vector3 directionToPlayer = playerHeadset.position - transform.position;

            // Check if the player successfully dodged it
            if (Vector3.Dot(transform.forward, directionToPlayer) < 0f)
            {
                hasPassedPlayer = true; // Break lock, fly straight forever
            }
            else
            {
                // Apply slight tracking
                Vector3 targetDirection = directionToPlayer.normalized;

                // Using Time.deltaTime instead of fixedDeltaTime since we are in Update now
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, trackingStrength * Time.deltaTime, 0.0f);
                transform.rotation = Quaternion.LookRotation(newDirection);
            }
        }

        // 3. Raycast Movement (Prevents clipping through walls & ignores physical rubble)
        float moveDistance = speed * Time.deltaTime; // 'speed' is inherited from parent

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, moveDistance, hitLayers))
        {
            HandleCollision(hit.collider);
        }
        else
        {
            transform.position += transform.forward * moveDistance;
        }

        // 4. Custom AR Headset distance check
        if (playerHeadset != null && isFlying)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerHeadset.position);
            if (distanceToPlayer <= hitRadius)
            {
                // Note: The headset usually doesn't have a GameStats script physically attached to the camera transform.
                // If you need it to apply damage directly to the player here, you can find your PlayerManager and apply it.
                HitTarget();
            }
        }
    }

    private void HandleCollision(Collider hitCollider)
    {
        isFlying = false;

        // If it hits an enemy/player with stats, apply the damage inherited from the parent
        if (hitCollider.TryGetComponent<GameStats>(out GameStats stats))
        {
            stats.GetDamage(damage);
        }

        // Call the destroy logic
        HitTarget();
    }

    // Override the parent's HitTarget to add our specific logging or effects
    protected override void HitTarget()
    {
        Debug.Log("AR Homing Projectile Hit or Collided!");
        isFlying = false;

        // This runs the Destroy(gameObject) from the parent script
        base.HitTarget();
    }
}