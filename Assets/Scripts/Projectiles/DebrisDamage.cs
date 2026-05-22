using UnityEngine;

public class DebrisDamage : MonoBehaviour
{
    [Tooltip("Base damage inherited from the Gravity Bullet.")]
    public int damageAmount = 10;

    [Tooltip("The object must be moving at least this fast to hurt someone.")]
    public float minVelocityToDamage = 3f;

    [Tooltip("The maximum damage this object can possibly do.")]
    public int maxDamageCap = 100;

    [Tooltip("Tags that this debris will NOT damage.")]
    public string[] safeTags = { "Player", "Projectile" };

    private Rigidbody rb;
    private bool initialized = false;

    // We use Awake instead of Start to bind the Rigidbody instantly when added!
    void Awake()
    {
        // Try finding Rigidbody on this object, or up on its parent structure
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = GetComponentInParent<Rigidbody>();
        }

        // Failsafe cleanup: Remove this component after 12 seconds of floating around
        Destroy(this, 12f);
    }

    void OnCollisionEnter(Collision collision)
    {
        // If we still can't find a physics body, this object can't calculate impact speed.
        if (rb == null)
        {
            Debug.LogWarning($"[DebrisDamage] {gameObject.name} has no Rigidbody found on self or parents. Removing script.");
            Destroy(this);
            return;
        }

        // Use linearVelocity for Unity 2022.2+ or velocity for older versions
        float impactSpeed = rb.linearVelocity.magnitude;

        // 1. Comprehensive Tag Filter: Abort if we hit a protected entity
        foreach (string tag in safeTags)
        {
            if (collision.gameObject.CompareTag(tag) || collision.transform.root.CompareTag(tag))
            {
                return; // Ignore players/projectiles entirely, but KEEP the script active!
            }
        }

        // 2. Minimum speed filter
        if (impactSpeed < minVelocityToDamage)
        {
            return; // Moving too slow, but KEEP the script active for later impacts!
        }

        // 3. Attempt to fetch GameStats from the target
        GameStats stats = collision.gameObject.GetComponent<GameStats>();
        if (stats == null)
        {
            stats = collision.gameObject.GetComponentInParent<GameStats>();
        }

        // 4. Hit Target Successfully!
        if (stats != null)
        {
            float speedMultiplier = impactSpeed / minVelocityToDamage;
            int scaledDamage = Mathf.RoundToInt(damageAmount * speedMultiplier);
            int finalDamage = Mathf.Clamp(scaledDamage, 0, maxDamageCap);

            stats.GetDamage(finalDamage);

            Debug.Log($"💥 [DEBRIS HIT] {gameObject.name} smashed {collision.gameObject.name} at {impactSpeed:F1} m/s for {finalDamage} damage!");

            // Self-destruct the script component now that it has successfully hit an enemy
            Destroy(this);
        }
        else
        {
             Debug.Log($"{gameObject.name} hit environment ({collision.gameObject.name}) and lost its charge.");
             Destroy(this);
        }
    }
}