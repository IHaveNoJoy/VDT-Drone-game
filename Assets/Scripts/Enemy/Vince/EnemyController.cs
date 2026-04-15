using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))] // ✅ ensure it exists
public class EnemyController : GameStats
{
    [Header("Target")]
    [SerializeField] private Transform target;

    private Rigidbody2D rb;
    private SpriteRenderer sr; // ✅ NEW

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>(); // ✅ NEW

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
            // ✅ Apply size
            transform.localScale = Vector3.one * data.size;

            // ✅ APPLY SPRITE (THIS WAS MISSING)
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

    public override void Kill()
    {
        Debug.Log($"{data.enemyName} Enemy Destroyed!");
        base.Kill();
    }
}