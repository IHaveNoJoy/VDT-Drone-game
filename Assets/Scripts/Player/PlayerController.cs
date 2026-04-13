using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : GameStats
{
    [Header("Flight Settings")]
    [SerializeField] private float flySpeed = 10f;
    [SerializeField] private float stabilizationSmoothing = 0.1f;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float fireRate = 0.15f; // Prevents "ghosting" if keys overlap

    private float nextFireTime;
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    // --- MOVEMENT ---
    // Keeping InputValue for movement as it is a continuous "Value" type
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector2 targetVelocity = moveInput * flySpeed;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, stabilizationSmoothing);

        if (moveInput.sqrMagnitude < 0.01f && rb.linearVelocity.sqrMagnitude < 0.01f)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // --- SHOOTING LOGIC ---
    // Switching to InputAction.CallbackContext to filter for the 'Started' phase (Initial Press)

    public void OnShoot_Left(InputAction.CallbackContext context)
    {
        if (context.started) SpawnProjectile(180f);
    }

    public void OnShoot_Right(InputAction.CallbackContext context)
    {
        if (context.started) SpawnProjectile(0f);
    }

    public void OnShoot_Above(InputAction.CallbackContext context)
    {
        if (context.started) SpawnProjectile(90f);
    }

    public void OnShoot_Under(InputAction.CallbackContext context)
    {
        if (context.started) SpawnProjectile(-90f);
    }

    private void SpawnProjectile(float angle)
    {
        // 1. Cooldown Check: Ensures the input system doesn't jam if multiple directions are hit
        if (Time.time < nextFireTime) return;
        if (projectilePrefab == null) return;

        nextFireTime = Time.time + fireRate;

        // 2. Spawn Logic
        Vector3 spawnPos = shootPoint != null ? shootPoint.position : transform.position;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        Instantiate(projectilePrefab, spawnPos, rotation);
    }

    public override void Kill()
    {
        Debug.Log("Drone Destroyed!");
        base.Kill();
    }
}