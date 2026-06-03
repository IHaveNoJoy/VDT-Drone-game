using UnityEngine;
using System.Collections.Generic; // Needed for the list to prevent double-damage

// Inherits from Projectile instead of MonoBehaviour!
public class GravitationalBullet : Projectile
{
    [Header("AR Targeting Settings")]
    public string targetTag = "zero";
    public float hitRadius = 0.3f;
    public string[] explodeOnTags = { "Player", "Projectiles" };
    public LayerMask hitLayers = ~0;

    [Header("Invisible Sphere (Gravity) Settings")]
    public float pullRadius = 5f;
    public float startingPullForce = 2f;
    public float maxPullForce = 40f;
    public float forceGrowthRate = 10f;
    public LayerMask pullableLayers = ~0;

    [Header("Explosion Settings")]
    [Tooltip("How hard the gathered debris gets pushed away when it explodes.")]
    public float explosionForce = 500f;

    private Transform targetTransform;
    private bool isMoving = true;
    private bool isPulling = true;
    private float currentPullForce;

    protected override void Start()
    {
        base.Start();

        currentPullForce = startingPullForce;

        GameObject targetObj = GameObject.FindGameObjectWithTag(targetTag);

        if (targetObj != null)
        {
            targetTransform = targetObj.transform;
            Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(directionToTarget);
        }
        else
        {
            Debug.LogError($"No object found with the tag '{targetTag}'.");
            Destroy(gameObject);
        }
    }

    protected override void Update()
    {
        timeElapsed += Time.deltaTime;

        if (isMoving && speed <= 0.01f)
        {
            Debug.Log("Failsafe Triggered: Speed reached zero. Forcing Explosion!");
            HitTarget();
            return;
        }

        if (isMoving)
        {
            float moveDistance = speed * Time.deltaTime;

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, moveDistance, hitLayers))
            {
                HandleCollision(hit.collider);
            }
            else
            {
                transform.position += transform.forward * moveDistance;
            }

            if (targetTransform != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);
                if (distanceToTarget <= hitRadius)
                {
                    HitTarget();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isPulling) return;

        if (currentPullForce < maxPullForce)
        {
            currentPullForce += forceGrowthRate * Time.fixedDeltaTime;
        }

        PullNearbyObjects();
    }

    private void PullNearbyObjects()
    {
        // Find everything in the pull radius
        Collider[] collidersToPull = Physics.OverlapSphere(transform.position, pullRadius, pullableLayers);

        foreach (Collider col in collidersToPull)
        {
            // Skip the bullet itself
            if (col.gameObject == gameObject) continue;

            Rigidbody targetRb = col.GetComponent<Rigidbody>();

            // Only pull and modify objects that actually have physics (Rigidbodies)
            if (targetRb != null)
            {
                Vector3 directionToBullet = transform.position - col.transform.position;
                float distance = directionToBullet.magnitude;

                if (distance > 0.1f)
                {
                    Vector3 pullDirection = directionToBullet.normalized;
                    targetRb.AddForce(pullDirection * currentPullForce, ForceMode.Acceleration);

                    {
                        if (col.gameObject.GetComponent<DebrisDamage>() == null)
                        {
                            DebrisDamage deadlyScript = col.gameObject.AddComponent<DebrisDamage>();

                            // Pass down parameters from the bullet
                            deadlyScript.damageAmount = this.damage;
                            deadlyScript.safeTags = explodeOnTags;

                            Debug.Log($"Successfully weaponized debris: {col.gameObject.name}!");
                        }
                    }
                }
            }
        }
    }

    private void HandleCollision(Collider hitCollider)
    {
        bool shouldExplode = false;

        foreach (string tag in explodeOnTags)
        {
            if (hitCollider.CompareTag(tag))
            {
                shouldExplode = true;
                break;
            }
        }

        if (shouldExplode)
        {
            HitTarget();
        }
        else
        {
            isMoving = false;
        }
    }

    protected override void HitTarget()
    {
        Debug.Log("Bullet Exploded!");
        isMoving = false;
        isPulling = false;

        TriggerExplosion();
        base.HitTarget();
    }

    private void TriggerExplosion()
    {
        float explosionRadius = pullRadius / 2f;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, hitLayers | pullableLayers);
        List<GameStats> damagedStats = new List<GameStats>();

        foreach (Collider col in hitColliders)
        {
            if (col.gameObject == gameObject) continue;

            Rigidbody targetRb = col.GetComponent<Rigidbody>();
            if (targetRb != null)
            {
                targetRb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 3f, ForceMode.Impulse);
            }

            GameStats stats = col.GetComponentInParent<GameStats>();
            if (stats != null && !damagedStats.Contains(stats))
            {
                stats.GetDamage(damage);
                damagedStats.Add(stats);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, pullRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pullRadius / 2f);
    }
}