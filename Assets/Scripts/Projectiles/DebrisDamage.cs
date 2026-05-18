using UnityEngine;

public class DebrisDamage : MonoBehaviour
{
    [Tooltip("Base damage inherited from the Gravity Bullet.")]
    public int damageAmount = 10;

    [Tooltip("The object must be moving at least this fast to hurt someone.")]
    public float minVelocityToDamage = 3f;

    [Tooltip("The maximum damage this object can possibly do (prevents physics glitches from instantly killing bosses).")]
    public int maxDamageCap = 100;

    [Tooltip("Tags that this debris will NOT damage (e.g., Player, Projectile).")]
    public string[] safeTags = { "Projectile" };

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Remove this script after 10 seconds so the debris becomes harmless again
        Destroy(this, 10f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (rb == null) return;

        // Get the speed of the object at the exact moment of impact
        float impactSpeed = rb.linearVelocity.magnitude;

        // 1. If it's barely moving, don't deal damage
        if (impactSpeed < minVelocityToDamage)
        {
            return;
        }

        // 2. Check if the object we hit is on the "Safe" list
        foreach (string tag in safeTags)
        {
            if (collision.gameObject.CompareTag(tag))
            {
                return; // Stop the code, do no damage
            }
        }

        // 3. If it hit an enemy, calculate the scaled damage!
        if (collision.gameObject.TryGetComponent<GameStats>(out GameStats stats))
        {
            // The Math: If it hits at exactly the minimum speed, it does 1x damage. 
            // If it hits at double the minimum speed, it does 2x damage, etc.
            float speedMultiplier = impactSpeed / minVelocityToDamage;
            int scaledDamage = Mathf.RoundToInt(damageAmount * speedMultiplier);

            // Enforce the damage cap so it doesn't break your game balance
            int finalDamage = Mathf.Clamp(scaledDamage, 0, maxDamageCap);

            // Deal the damage
            stats.GetDamage(finalDamage);

            // Log it so you can see the speed and damage in the console!
            Debug.Log($"{gameObject.name} smashed into {collision.gameObject.name} at {impactSpeed:F1}m/s for {finalDamage} damage!");

            // Optional: Destroy the debris after it hits an enemy so it shatters
            // Destroy(gameObject); 
        }
    }
}