using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public float speed = 20f;
    public float lifeSpan = 5f;

    private void Start()
    {
        Destroy(gameObject, lifeSpan);
    }

    private void Update()
    {
        transform.Translate(
            Vector3.forward * speed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<GameStats>(out GameStats stats))
        {
            stats.GetDamage(damage);

            Destroy(gameObject);
        }
    }
}