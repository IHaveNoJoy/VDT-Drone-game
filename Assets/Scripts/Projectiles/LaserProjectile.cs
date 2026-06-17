using UnityEngine;

public class LaserProjectile : Projectile
{
    [Header("Laser & Explosion Settings")]
    [Tooltip("Drag the child TinyExplosion object from your hierarchy into this slot.")]
    public GameObject explosionPrefab;

    [Tooltip("If greater than 0, the laser does Area of Effect (AoE) damage when it explodes.")]
    public float explosionRadius = 0f;

    [Tooltip("Splash damage dealt to objects caught in the explosion radius.")]
    public int explosionDamage = 10;

    [HideInInspector] public GameObject owner; // ✅ IMPORTANT: who fired it

    private bool hasExploded = false;

    protected override void Start()
    {
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
            explosionPrefab.transform.SetParent(null);
            explosionPrefab.SetActive(true);
            Destroy(explosionPrefab, 3f);
        }

        // ✅ AOe DAMAGE
        if (explosionRadius > 0f)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider hit in colliders)
            {
                // ✅ ignore owner completely
                if (owner != null && hit.gameObject == owner) 
                    continue;

                if (hit.TryGetComponent<GameStats>(out GameStats stats))
                {
                    stats.GetDamage(explosionDamage);
                }
            }
        }
    }
}