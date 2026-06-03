using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Base Projectile Settings")]
    public int damage = 10;
    public float speed = 20f;
    public float lifeSpan = 5f;

    [Header("Arming Logic")]
    [SerializeField] protected float armingTime = 0.5f;
    protected float timeElapsed = 0f;

    // Use 'protected virtual' so child scripts can override or add to these!
    protected virtual void Start()
    {
        // Automatically destroy after lifeSpan ends
        Destroy(gameObject, lifeSpan);
    }

    protected virtual void Update()
    {
        // Track how long the bullet has been alive
        timeElapsed += Time.deltaTime;

        // Standard 3D movement forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    // Changed to 3D Physics (Collider instead of Collider2D)
    protected virtual void OnTriggerStay(Collider collision)
    {
        if (collision == null) return;

        // 1. Wait until armed
        if (timeElapsed < armingTime) return;

        // 2. Once armed, check for stats
        if (collision.TryGetComponent<GameStats>(out GameStats stats))
        {
            stats.GetDamage(damage);
            HitTarget();
        }
    }

    protected virtual void HitTarget()
    {
        Destroy(gameObject);
    }
}