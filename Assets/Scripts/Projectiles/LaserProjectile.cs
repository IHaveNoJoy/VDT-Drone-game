using UnityEngine;

public class LaserProjectile : Projectile
{
    [Header("Laser & Explosion Settings")]
    [Tooltip("Drag the child TinyExplosion object from your hierarchy into this slot.")]
    public GameObject explosionPrefab; // In your case, this is the child object

    [Tooltip("If greater than 0, the laser does Area of Effect (AoE) damage when it explodes.")]
    public float explosionRadius = 0f;

    [Tooltip("Splash damage dealt to objects caught in the explosion radius.")]
    public int explosionDamage = 10;

    private bool hasExploded = false;

    protected override void Start()
    {
        // Deactivates the explosion on start so it doesn't play while flying
        if (explosionPrefab != null)
        {
            explosionPrefab.SetActive(false);
        }

        Invoke(nameof(ExplodeAndDestroy), lifeSpan);
    }

    protected override void HitTarget()
    {
        Explode();
        base.HitTarget();
    }

    private void ExplodeAndDestroy()
    {
        Explode();
        Destroy(gameObject);
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        if (explosionPrefab != null)
        {
            // 1. Cut the cord: Detach the explosion from the parent so it doesn't get destroyed with the laser
            explosionPrefab.transform.SetParent(null);

            // 2. Turn it on so the particles/visuals actually play
            explosionPrefab.SetActive(true);

            // 3. Tell the explosion to delete itself from the world in 3 seconds
            Destroy(explosionPrefab, 3f);
        }

        // Apply optional Area of Effect (AoE) splash damage
        if (explosionRadius > 0f)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider hit in colliders)
            {
                if (hit.TryGetComponent<GameStats>(out GameStats stats))
                {
                    stats.GetDamage(explosionDamage);
                }
            }
        }
    }
}