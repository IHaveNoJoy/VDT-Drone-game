using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : GameStats
{
    [Header("Network Settings")]
    [Tooltip("Must match the Inbound Key in WSHost (e.g., Drone1)")]
    public string droneNetworkKey = "Drone1";

    [Header("Drone Navigation")]
    [Tooltip("Drag the Target Box GameObject here")]
    [SerializeField] private Transform targetBox;

    [Header("Flight Physics")]
    [SerializeField] private float thrustPower = 15f;      // How hard the drone tries to reach the target
    [SerializeField] private float artificialDrag = 3f;    // Braking power / air resistance
    [SerializeField] private float maxSpeed = 12f;         // Terminal velocity

    [Header("Shooting Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float fireRate = 0.15f;
    public LaserController myLaser;

    private float nextFireTime;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Give it a little gravity so it "falls" if it gets too far off course, 
        // mimicking a drone losing lift. Adjust to taste.
        rb.gravityScale = 0.5f;
        rb.freezeRotation = true;

        // Set Rigidbody linear drag to 0 in inspector; we handle drag via code for better control
        rb.linearDamping = 0f;
    }

    private void Update()
    {
        // Handle GameStats updates (like health regeneration or invincibility timers)
        base.Update();
    }

    private void FixedUpdate()
    {
        // --- 1. NETWORK MODE: PHYSICAL DRONE CHECK ---
        if (WSHost.Instance != null && WSHost.Instance.HasData(droneNetworkKey))
        {
            // Stop Unity from simulating physics while the real drone is flying
            rb.isKinematic = true;

            // Get the 3D position from the real world via WebSocket
            Vector3 realPos = WSHost.Instance.getPosition(droneNetworkKey) * WSHost.Instance.Factor;

            // Convert real-world 3D (X, Z) into Unity 2D (X, Y)
            transform.position = new Vector2(realPos.x, realPos.z);

            // Apply real-world rotation mapped to 2D
            float realYaw = WSHost.Instance.getYaw(droneNetworkKey);
            transform.rotation = Quaternion.Euler(0f, 0f, -realYaw);

            return; // Exit here so we don't apply the default physics
        }

        // --- 2. DEFAULT MODE: VIRTUAL DRONE PHYSICS ---
        if (rb.isKinematic)
        {
            rb.isKinematic = false; // Turn physics back on if the real drone disconnects
        }

        if (targetBox == null) return;

        // 1. Calculate the vector pointing from the drone to the target box
        Vector2 directionToTarget = (Vector2)targetBox.position - rb.position;

        // 2. Apply Thrust (Spring Force)
        Vector2 thrustForce = directionToTarget * thrustPower;

        // 3. Apply Damping (Air Friction)
        Vector2 dampingForce = -rb.linearVelocity * artificialDrag;

        // 4. THE FIX: Gravity Compensation (Hover Throttle)
        // This calculates the exact downward force of gravity on this specific rigidbody and creates an equal upward force
        Vector2 gravityCompensation = -Physics2D.gravity * rb.gravityScale * rb.mass;

        // 5. Execute physical movement (Notice we added gravityCompensation here)
        rb.AddForce(thrustForce + dampingForce + gravityCompensation);

        // 6. Clamp speed to ensure it doesn't break the sound barrier
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    // --- SHOOTING & HEALTH LOGIC (Unchanged) --- //

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

    public void OnFireLaser(InputValue value)
    {
        myLaser.FireLaser();
    }

    private void SpawnProjectile(float angle)
    {
        if (Time.time < nextFireTime || projectilePrefab == null) return;

        nextFireTime = Time.time + fireRate;

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