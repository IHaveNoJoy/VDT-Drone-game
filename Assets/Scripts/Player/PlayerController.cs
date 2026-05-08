using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : GameStats
{
    public bool isPlayer1;

    [Header("Flight Settings")]
    [SerializeField] public float flySpeed = 10f;
    [SerializeField] public float stabilizationSmoothing = 0.1f;

    [Header("Shooting Settings")]
    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] public Transform shootPoint;
    [SerializeField] public float fireRate = 0.15f; // Prevents "ghosting" if keys overlap

    [HideInInspector]
    public float nextFireTime;
    public Rigidbody2D rb;
    public Vector2 moveInput;

    public KeyCode up;
    public KeyCode right;
    public KeyCode left;
    public KeyCode down;

    public KeyCode shoot;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (!isPlayer1)
        {
            transform.localRotation *= Quaternion.Euler(0, 0, 180);
        }
    }


    public void OnMove(InputValue value)
    {
        if (isPlayer1) { moveInput = value.Get<Vector2>(); 
        }
    }

    public virtual void FixedUpdate()
    {
        if (!isPlayer1)
        {
            float x = 0;
            float y = 0;
            if (Input.GetKey(up)) { y = 1; }
            if (Input.GetKey(down)) { y = -1; }
            if (Input.GetKey(right)) { x = 1; }
            if (Input.GetKey(left)) { x = -1; }

            moveInput = new Vector2(x, y);
        }


        Vector2 targetVelocity = moveInput * flySpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, stabilizationSmoothing);

        if (moveInput.sqrMagnitude < 0.01f && rb.linearVelocity.sqrMagnitude < 0.01f)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }



    public virtual void OnShoot_Left(InputValue value)
    {
        Debug.Log("Hello");
        if (value.isPressed) SpawnProjectile(180f, shootPoint);
    }

    public virtual void OnShoot_Right(InputValue value)
    {
        Debug.Log("Shoot right!");
        if (value.isPressed) SpawnProjectile(0f, shootPoint);
    }

    public virtual void OnShoot_Above(InputValue value)
    {
        if (value.isPressed) SpawnProjectile(90f, shootPoint);
    }

    public virtual void OnShoot_Under(InputValue value)
    {
        if (value.isPressed) SpawnProjectile(-90f, shootPoint);
    }

    public virtual void SpawnProjectile(float angle, Transform pos)
    {
        // 1. Cooldown Check: Ensures the input system doesn't jam if multiple directions are hit
        if (Time.time < nextFireTime) return;
        if (projectilePrefab == null) return;

        nextFireTime = Time.time + fireRate;

        // 2. Spawn Logic
        Vector3 spawnPos = pos != null ? pos.position : transform.position;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        Instantiate(projectilePrefab, spawnPos, rotation);
    }

    public override void Kill()
    {
        Debug.Log("Drone Destroyed!");
        base.Kill();
    }
}