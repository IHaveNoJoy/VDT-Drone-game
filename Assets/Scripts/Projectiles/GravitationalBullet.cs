using UnityEngine;
using System.Collections.Generic; // Needed for the list to prevent double-damage

// Inherits from Projectile instead of MonoBehaviour!
public class GravitationalBullet : Projectile
{
    [Header("AR Targeting Settings")]
    public string targetTag = "zero";
    public float hitRadius = 0.3f;
    public string[] explodeOnTags = { "Player", "Projectile" };
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
            return; // Exit out of the Update loop early so it doesn't try to calculate movement
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

            // Custom AR Headset proximity check
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
        Collider[] collidersToPull = Physics.OverlapSphere(transform.position, pullRadius, pullableLayers);

        foreach (Collider col in collidersToPull)
        {
            if (col.gameObject == gameObject) continue;

            Rigidbody targetRb = col.GetComponent<Rigidbody>();

            if (targetRb != null)
            {
                Vector3 directionToBullet = transform.position - col.transform.position;
                float distance = directionToBullet.magnitude;

                if (distance > 0.1f)
                {
                    Vector3 pullDirection = directionToBullet.normalized;
                    targetRb.AddForce(pullDirection * currentPullForce, ForceMode.Acceleration);

                    // Attach the script to the rubble!
                    if (col.gameObject.GetComponent<DebrisDamage>() == null && !col.gameObject.CompareTag("Player"))
                    {
                        DebrisDamage deadlyScript = col.gameObject.AddComponent<DebrisDamage>();
                        deadlyScript.damageAmount = this.damage;
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
            // We removed the direct damage here, because the explosion will now handle hitting the targets!
            HitTarget();
        }
        else
        {
            // Hit a normal wall, stick to it and keep pulling
            isMoving = false;
        }
    }

    // This overrides the parent script to create the explosion before destroying!
    protected override void HitTarget()
    {
        Debug.Log("Bullet Exploded!");
        isMoving = false;
        isPulling = false;

        TriggerExplosion();

        // This runs the Destroy(gameObject) from the parent script
        base.HitTarget();
    }

    private void TriggerExplosion()
    {
        // 1. Calculate the explosion radius (Half of the gravity pull radius)
        float explosionRadius = pullRadius / 2f;

        // 2. Find everything inside the explosion
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius, hitLayers | pullableLayers);

        // Keep a list of targets we already damaged so we don't hit a boss twice if it has two colliders (like arms and body)
        List<GameStats> damagedStats = new List<GameStats>();

        foreach (Collider col in hitColliders)
        {
            if (col.gameObject == gameObject) continue;

            // --- PUSH DEBRIS AWAY ---
            Rigidbody targetRb = col.GetComponent<Rigidbody>();
            if (targetRb != null)
            {
                // Unity has a built-in method just for this!
                // ForceMode.Impulse creates an instant "bang" rather than a slow push
                targetRb.AddExplosionForce(explosionForce, transform.position, explosionRadius, 3f, ForceMode.Impulse);
            }

            // --- DEAL AREA DAMAGE ---
            // We use GetComponentInParent just in case the raycast hits an arm, we want to damage the main body
            GameStats stats = col.GetComponentInParent<GameStats>();
            if (stats != null && !damagedStats.Contains(stats))
            {
                stats.GetDamage(damage);
                damagedStats.Add(stats); // Add them to the list so they don't get double-damaged
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Magenta for the Gravity Pull
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, pullRadius);

        // Red for the Explosion Impact Area (Half the size)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pullRadius / 2f);
    }
}