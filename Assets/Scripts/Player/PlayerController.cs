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


    public void OnMove(InputValue value)
    {
 
        moveInput = value.Get<Vector2>();
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



    public void OnShoot_Left(InputValue value)
    {
        Debug.Log("Hello");
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

        GameObject bullet = Instantiate(projectilePrefab, spawnPos, rotation);

        Projectile proj = bullet.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.owner = this; //assign owner to bullet to avoid self damage
        }
    }

    public override void Kill()
    {
        Debug.Log("Drone Destroyed!");
        base.Kill();
    }
}