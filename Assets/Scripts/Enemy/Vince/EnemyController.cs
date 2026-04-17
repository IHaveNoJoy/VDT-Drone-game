using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyController : GameStats
{
    [Header("Target")]
    [SerializeField] private Transform target;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private float nextDamageTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    public override void Start()
    {
        base.Start();

        if (target == null)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
                target = player.transform;
        }

        if (data != null)
        {
            transform.localScale = Vector3.one * data.size;

            if (sr != null && data.sprite != null)
            {
                sr.sprite = data.sprite;
            }
        }
    }

    private void FixedUpdate()
    {
        if (target == null || data == null) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * data.moveSpeed;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (data == null) return;

        // cooldown check
        if (Time.time < nextDamageTime) return;

        if (collision.gameObject.TryGetComponent<PlayerController>(out PlayerController player))
        {
            if (player.IsInvulnerable) return;
            Debug.Log("Enemy hit player"); // ✅ debug proof

            player.GetDamage(data.contactDamage);

            nextDamageTime = Time.time + data.damageCooldown;
        }
    }

    public override void Kill()
    {
        Debug.Log($"{data.enemyName} Enemy Destroyed!");
        base.Kill();
    }
}