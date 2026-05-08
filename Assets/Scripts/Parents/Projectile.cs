using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public float damage = 10;
    public float speed = 20f;
    public float lifeSpan = 5f;
    public float size = 1f;

    [Header("Ownership")]
    public GameStats owner;

    public virtual void Start()
    {
        Destroy(gameObject, lifeSpan);
    }

    public virtual void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<GameStats>(out GameStats stats))
            return;

        // Ignore the owner (this already exists but keep it)
        if (stats == owner)
            return;

        // EXTRA SAFETY: prevent hitting same "type"
        if (owner is PlayerController && stats is PlayerController)
            return;

        if (owner is EnemyController && stats is EnemyController)
            return;

        // ✅ valid hit
        stats.GetDamage(damage);
        HitTarget();
    }

    protected virtual void HitTarget()
    {
        Destroy(gameObject);
    }
}