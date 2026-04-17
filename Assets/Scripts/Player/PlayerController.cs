using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : GameStats
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;

    [Header("BOOST (Stamina Sprint)")]
    [SerializeField] private float boostMultiplier = 3f;
    [SerializeField] private float maxStamina = 20f;
    [SerializeField] private float staminaDrainRate = 2f;
    [SerializeField] private float staminaRegenRate = 1.5f;

    [Header("DASH (Charge)")]
    [SerializeField] private float dashSpeed = 80f;
    [SerializeField] private float dashDuration = 0.25f;
    [SerializeField] private float dashCooldown = 8f;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float fireRate = 0.15f;

    private float nextFireTime;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    // BOOST
    private float stamina;
    private bool isBoosting;

    // DASH
    private bool isDashing;
    private float dashEndTime;
    private float nextDashTime;
    private Vector2 dashDirection;

    // INVULNERABILITY
    public bool IsInvulnerable { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        stamina = maxStamina;
    }

    // ---------------- MOVEMENT ----------------

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnBoost(InputValue value)
    {
        isBoosting = !isBoosting;
    }

    public void OnDash(InputValue value)
    {
        if (!value.isPressed) return;
        if (Time.time < nextDashTime) return;

        isDashing = true;
        dashEndTime = Time.time + dashDuration;
        nextDashTime = Time.time + dashCooldown;

        dashDirection = moveInput.sqrMagnitude > 0.01f
            ? moveInput.normalized
            : Vector2.right;

        IsInvulnerable = true;
    }

    private void FixedUpdate()
    {
        HandleStamina();

        if (isDashing)
        {
            HandleDash();
            return;
        }

        HandleMovement();
    }

    private void HandleMovement()
    {
        float speed = moveSpeed;

        if (isBoosting && stamina > 0f)
        {
            speed *= boostMultiplier;
            stamina -= staminaDrainRate * Time.fixedDeltaTime;
            stamina = Mathf.Max(0f, stamina);
        }

        rb.linearVelocity = moveInput * speed;
    }

    private void HandleStamina()
    {
        if (!isBoosting)
        {
            stamina += staminaRegenRate * Time.fixedDeltaTime;
            stamina = Mathf.Min(maxStamina, stamina);
        }
    }

    private void HandleDash()
    {
        rb.linearVelocity = dashDirection * dashSpeed;

        if (Time.time >= dashEndTime)
        {
            isDashing = false;
        if (Time.time >= dashEndTime * 3)
            IsInvulnerable = false;
        }
    }

    // ---------------- SHOOTING ----------------

    public void OnShoot_Left(InputValue value)
    {
        if (value.isPressed) SpawnProjectile(180f);
    }

    public void OnShoot_Right(InputValue value)
    {
        if (value.isPressed) SpawnProjectile(0f);
    }

    public void OnShoot_Above(InputValue value)
    {
        if (value.isPressed) SpawnProjectile(90f);
    }

    public void OnShoot_Under(InputValue value)
    {
        if (value.isPressed) SpawnProjectile(-90f);
    }

    private void SpawnProjectile(float angle)
    {
        if (Time.time < nextFireTime) return;
        if (projectilePrefab == null) return;

        nextFireTime = Time.time + fireRate;

        Vector3 spawnPos = shootPoint != null ? shootPoint.position : transform.position;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        Instantiate(projectilePrefab, spawnPos, rotation);
    }

    // ---------------- DAMAGE ----------------

    public override void GetDamage(int Damage)
    {
        if (IsInvulnerable)
            return;

        base.GetDamage(Damage);
    }

    public override void Kill()
    {
        Debug.Log("Player destroyed");
        base.Kill();
    }
}