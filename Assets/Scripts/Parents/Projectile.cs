using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    public int damage = 10;
    public float speed = 20f;
    public float lifeSpan = 5f;

    [Header("Arming Logic")]
    [SerializeField] private float armingTime = 0.25f;
    private float timeElapsed = 0f;

    protected virtual void Start()
    {
        Destroy(gameObject, lifeSpan);
    }

    protected virtual void Update()
    {
        // Track how long the bullet has been alive
        timeElapsed += Time.deltaTime;

        // Standard movement
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
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
        Destroy(this.gameObject);
    }
}