using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public int damage = 10;
    public float speed = 20f;
    public float lifeSpan = 5f;

    [Header("Ownership")]
    public GameStats owner; // 🚀 NEW: who fired this bullet

    protected virtual void Start()
    {
        Destroy(gameObject, lifeSpan);
    }

    protected virtual void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<GameStats>(out GameStats stats))
        {
            // 🚫 prevent self-hit (spawn protection solved properly)
            if (stats == owner) return;

            stats.GetDamage(damage);
            HitTarget();
        }
    }

    protected virtual void HitTarget()
    {
        Destroy(gameObject);
    }
}