using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Base Projectile Settings")]
    public int damage = 10;
    public float speed = 20f;
    public float lifeSpan = 5f;

    [Header("Arming Logic")]
    [SerializeField] protected float armingTime = 0.25f;
    protected float timeElapsed = 0f;

    // Use 'protected virtual' so child scripts can override or add to these!
    protected virtual void Start()
    {
        // Automatically destroy after lifeSpan ends
        Destroy(gameObject, lifeSpan);
    }

    private void Update()
    {
        // Track how long the bullet has been alive
        timeElapsed += Time.deltaTime;

        // Standard 3D movement forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    // Changed to 3D Physics (Collider instead of Collider2D)
    protected virtual void OnTriggerStay(Collider collision)
    {
        if (other.TryGetComponent<GameStats>(out GameStats stats))
        {
            stats.GetDamage(damage);

            Destroy(gameObject);
        }
    }

    protected virtual void HitTarget()
    {
        Destroy(gameObject);
    }
}