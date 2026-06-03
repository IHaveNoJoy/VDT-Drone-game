using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : GameStats
{
    [Header("Network Settings")]
    [Tooltip("Must match the Inbound Key in WSHost (e.g., Drone1)")]
    public string droneNetworkKey = "Drone1";

    [Header("Drone Navigation")]
    [Tooltip("Drag the Target Box GameObject here")]
    [SerializeField] private Transform targetBox;

    [Header("Flight Physics")]
    [SerializeField] private float thrustPower = 15f;
    [SerializeField] private float artificialDrag = 3f;
    [SerializeField] private float maxSpeed = 12f;

    [Tooltip("Replaces Rigidbody2D's gravityScale to control how fast it falls off course")]
    [SerializeField] private float customGravityScale = 0.5f;

    [Header("Legacy Shooting Settings")]
    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] public Transform shootPoint;

    [Header("Advanced Weapon Systems")]
    [Tooltip("Prefab for your main laser bolts")]
    public GameObject laserPrefab;
    [Tooltip("Drag Laser1.1, Laser1.2, etc. here")]
    public Transform[] laserShootPoints;

    [Space(10)]
    [Tooltip("Prefab for your rockets/missiles")]
    public GameObject rocketPrefab;
    [Tooltip("Drag Rocket1 etc. here")]
    public Transform[] rocketShootPoints;

    [Header("Fire Rates & Aiming")]
    [SerializeField] public float fireRate = 0.15f;
    [SerializeField] public float rocketFireRate = 1.0f;
    [Tooltip("Offset angle (0 = Straight ahead from the shoot point)")]
    [SerializeField] public float shootAngle = 0f;

    [HideInInspector] public float nextFireTime;
    private float nextRocketFireTime;

    public Rigidbody rb;

    // legacy
    public bool isPlayer1;
    public bool isPlayer2;
    public KeyCode shoot;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.linearDamping = 0f;
    }

    private void Update()
    {
        base.Update();
    }

    public virtual void FixedUpdate()
    {
        // --- 1. NETWORK MODE ---
        if (WSHost.Instance != null && WSHost.Instance.HasData(droneNetworkKey))
        {
            rb.isKinematic = true;
            Vector3 realPos = WSHost.Instance.getPosition(droneNetworkKey) * WSHost.Instance.Factor;
            transform.position = realPos;
            float realYaw = WSHost.Instance.getYaw(droneNetworkKey);
            transform.rotation = Quaternion.Euler(0f, -realYaw, 0f);
            return;
        }

        // --- 2. VIRTUAL DRONE PHYSICS ---
        if (rb.isKinematic) rb.isKinematic = false;

        Vector3 appliedGravity = Physics.gravity * customGravityScale;
        rb.AddForce(appliedGravity, ForceMode.Acceleration);

        if (targetBox == null) return;

        Vector3 directionToTarget = targetBox.position - rb.position;
        Vector3 thrustForce = directionToTarget * thrustPower;
        Vector3 dampingForce = -rb.linearVelocity * artificialDrag;
        Vector3 gravityCompensation = -appliedGravity * rb.mass;

        rb.AddForce(thrustForce + dampingForce + gravityCompensation);

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    // --- TRIGGER COMMANDS EXECUTION (CALLED VIA TARGET) ---

    public void CommandFireLasers()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        // Apply a clean local angle offset around the Y-axis if you have a shootAngle set up
        Quaternion offset = Quaternion.Euler(0, shootAngle, 0);

        if (laserShootPoints != null && laserShootPoints.Length > 0 && laserPrefab != null)
        {
            foreach (Transform point in laserShootPoints)
            {
                // Multiply the point's natural rotation by your offset so it shoots perfectly straight out of the muzzle's forward direction!
                Instantiate(laserPrefab, point.position, point.rotation * offset);
            }
        }
        else if (projectilePrefab != null)
        {
            Vector3 spawnPos = shootPoint != null ? shootPoint.position : transform.position;
            Quaternion spawnRot = shootPoint != null ? shootPoint.rotation * offset : transform.rotation * offset;

            Instantiate(projectilePrefab, spawnPos, spawnRot);
        }
    }

    public void CommandFireRockets()
    {
        if (Time.time < nextRocketFireTime || rocketPrefab == null || rocketShootPoints == null || rocketShootPoints.Length == 0) return;

        nextRocketFireTime = Time.time + rocketFireRate;
        Quaternion offset = Quaternion.Euler(0, shootAngle, 0);

        foreach (Transform point in rocketShootPoints)
        {
            // Shoots straight forward out of your rocket launcher nozzles!
            Instantiate(rocketPrefab, point.position, point.rotation * offset);
        }
    }

    public override void Kill()
    {
        Debug.Log("Drone Destroyed!");
        base.Kill();
    }
}