using UnityEngine;
using System.Collections.Generic;

public class GravitationalBullet2 : Projectile
{
    [Header("AR Targeting Settings")]
    public string targetTag = "zero";
    public float hitRadius = 0.3f;
    public string[] explodeOnTags = { "Player", "Projectiles" };
    public LayerMask hitLayers = ~0;

    [Header("Homing Settings")]
    public bool enableHoming = true;
    public float homingDelay = 0.4f;
    public float turnSpeed = 5f;

    [Header("Invisible Sphere (Gravity) Settings")]
    public float pullRadius = 5f;
    public float startingPullForce = 2f;
    public float maxPullForce = 40f;
    public float forceGrowthRate = 10f;
    public LayerMask pullableLayers = ~0;

    [Header("Explosion Settings")]
    public float explosionForce = 500f;

    private Transform targetTransform;
    private bool isMoving = true;
    private bool isPulling = true;
    private float currentPullForce;

    private bool homingActive = false;

    protected override void Start()
    {
        base.Start();

        currentPullForce = startingPullForce;

        GameObject targetObj = GameObject.FindGameObjectWithTag(targetTag);

        if (targetObj != null)
        {
            targetTransform = targetObj.transform;

            // ❌ IMPORTANT FIX:
            // DO NOT override rotation here anymore
            // Let Boss pattern define direction
        }
        else
        {
            Debug.LogError($"No object found with the tag '{targetTag}'.");
            Destroy(gameObject);
        }

        // Start delayed homing
        if (enableHoming)
        {
            Invoke(nameof(EnableHoming), homingDelay);
        }
    }

    void EnableHoming()
    {
        homingActive = true;
    }

    protected override void Update()
    {
        timeElapsed += Time.deltaTime;

        if (!isMoving) return;

        float moveDistance = speed * Time.deltaTime;

        // Optional homing AFTER delay
        if (homingActive && targetTransform != null)
        {
            Vector3 direction = (targetTransform.position - transform.position).normalized;

            Quaternion targetRot = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                turnSpeed * Time.deltaTime
            );
        }

        // Movement
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, moveDistance, hitLayers))
        {
            HandleCollision(hit.collider);
        }
        else
        {
            transform.position += transform.forward * moveDistance;
        }

        // Proximity hit
        if (targetTransform != null)
        {
            if (Vector3.Distance(transform.position, targetTransform.position) <= hitRadius)
            {
                HitTarget();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isPulling) return;

        if (currentPullForce < maxPullForce)
            currentPullForce += forceGrowthRate * Time.fixedDeltaTime;

        PullNearbyObjects();
    }

    private void PullNearbyObjects()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, pullRadius, pullableLayers);

        foreach (var col in cols)
        {
            if (col.gameObject == gameObject) continue;

            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = transform.position - col.transform.position;
                float dist = dir.magnitude;

                if (dist > 0.1f)
                {
                    rb.AddForce(dir.normalized * currentPullForce, ForceMode.Acceleration);

                    if (col.GetComponent<DebrisDamage>() == null)
                    {
                        var dmg = col.gameObject.AddComponent<DebrisDamage>();
                        dmg.damageAmount = damage;
                        dmg.safeTags = explodeOnTags;
                    }
                }
            }
        }
    }

    private void HandleCollision(Collider hitCollider)
    {
        foreach (string tag in explodeOnTags)
        {
            if (hitCollider.CompareTag(tag))
            {
                HitTarget();
                return;
            }
        }

        isMoving = false;
    }

    protected override void HitTarget()
    {
        isMoving = false;
        isPulling = false;

        TriggerExplosion();
        base.HitTarget();
    }

    private void TriggerExplosion()
    {
        float radius = pullRadius / 2f;

        Collider[] hits = Physics.OverlapSphere(transform.position, radius, hitLayers | pullableLayers);
        List<GameStats> damaged = new List<GameStats>();

        foreach (var col in hits)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddExplosionForce(explosionForce, transform.position, radius);

            GameStats stats = col.GetComponentInParent<GameStats>();

            if (stats != null && !damaged.Contains(stats))
            {
                stats.GetDamage(damage);
                damaged.Add(stats);
            }
        }
    }
}