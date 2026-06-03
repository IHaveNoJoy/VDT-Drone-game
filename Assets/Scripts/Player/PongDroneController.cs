using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PongDroneController : MonoBehaviour
{
    [Header("Player Side Setup")]
    public bool isFacingRight = true;

    [Header("Drone Simulation")]
    public Transform targetCube;
    public float targetSpeed = 15f;
    public float droneFollowForce = 10f;
    public float stopDistance = 0.05f;

    [Header("Movement Zone")]
    public Vector2 minBounds = new Vector2(-8f, -4.5f);
    public Vector2 maxBounds = new Vector2(-2f, 4.5f);

    [Header("Bat & Swing Settings")]
    public Transform batPivot;
    public float swingSpeed = 1200f;
    public float startAngle = 180f;
    public float endAngle = 0f;

    [Header("Charge Mechanics")]
    public float quickSwingMultiplier = 1.25f;
    public float maxChargeMultiplier = 3.0f;
    public float chargeTimePerLevel = 0.5f;
    public float multiplierIncreasePerLevel = 0.5f;

    [Header("Visuals")]
    public GameObject chargeParticlePrefab;
    public Transform effectSpawnPoint;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isChargeButtonPressed;
    private bool isSwinging = false;
    private bool swingingForward = false;

    // NEW: Variables to hold the direction-corrected angles
    private float adjustedStartAngle;
    private float adjustedEndAngle;
    private float currentBatAngle = 0f;

    private bool isCharging = false;
    private float chargeTimer = 0f;
    private float currentMultiplier = 1f;
    private float activeSwingMultiplier = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        // Apply the flip once at start
        if (batPivot != null)
        {
            batPivot.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);

            // Set initial rotation
            currentBatAngle = GetCorrectedAngle(startAngle);
            batPivot.localRotation = Quaternion.Euler(0, 0, currentBatAngle);
        }

        if (targetCube != null) targetCube.SetParent(null);
        else
        {
            GameObject vt = new GameObject("VirtualTargetCube");
            targetCube = vt.transform;
            targetCube.position = transform.position;
        }

        // Handle target cube
        if (targetCube != null) targetCube.SetParent(null);
        else
        {
            GameObject vt = new GameObject("VirtualTargetCube");
            targetCube = vt.transform;
            targetCube.position = transform.position;
        }
    }

    // ... (OnMove and OnSwing remain the same)
    void OnMove(InputValue value)
    {
        Vector2 raw = value.Get<Vector2>();
        moveInput = (raw.magnitude < 0.1f) ? Vector2.zero : raw;
    }

    void OnSwing(InputValue value)
    {
        isChargeButtonPressed = value.Get<float>() > 0.5f;
    }

    void Update()
    {
        HandleTargetInputAndBounds();
        HandleBatInputAndCharge();
        AnimateBatSwing();
    }

    void FixedUpdate()
    {
        Vector2 dir = targetCube.position - transform.position;
        if (dir.magnitude < stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            transform.position = targetCube.position;
        }
        else
        {
            rb.linearVelocity = dir * droneFollowForce;
        }
    }

    private void HandleTargetInputAndBounds()
    {
        Vector3 newPos = targetCube.position + (new Vector3(moveInput.x, moveInput.y, 0) * targetSpeed * Time.deltaTime);
        newPos.x = Mathf.Clamp(newPos.x, minBounds.x, maxBounds.x);
        newPos.y = Mathf.Clamp(newPos.y, minBounds.y, maxBounds.y);
        targetCube.position = newPos;
    }

    private void HandleBatInputAndCharge()
    {
        if (isSwinging) return;

        if (isChargeButtonPressed && !isCharging)
        {
            isCharging = true;
            chargeTimer = 0f;
            currentMultiplier = quickSwingMultiplier;
        }

        if (isCharging && isChargeButtonPressed)
        {
            chargeTimer += Time.deltaTime;
            if (chargeTimer >= chargeTimePerLevel && currentMultiplier < maxChargeMultiplier)
            {
                chargeTimer = 0f;
                currentMultiplier += multiplierIncreasePerLevel;

                // Use the custom spawn point if assigned, otherwise default to pivot
                Vector3 spawnPos = (effectSpawnPoint != null) ? effectSpawnPoint.position : batPivot.position;
                Instantiate(chargeParticlePrefab, spawnPos, Quaternion.identity);
            }
        }

        if (!isChargeButtonPressed && isCharging)
        {
            isCharging = false;
            isSwinging = true;
            swingingForward = true;
            activeSwingMultiplier = currentMultiplier;
        }
    }

    private void AnimateBatSwing()
    {
        if (!isSwinging) return;

        // Calculate targets on the fly so inspector changes apply immediately
        float currentTarget = swingingForward ? GetCorrectedAngle(endAngle) : GetCorrectedAngle(startAngle);

        currentBatAngle = Mathf.MoveTowards(currentBatAngle, currentTarget, swingSpeed * Time.deltaTime);

        if (batPivot != null) batPivot.localRotation = Quaternion.Euler(0, 0, currentBatAngle);

        if (Mathf.Abs(currentBatAngle - currentTarget) < 0.1f)
        {
            if (swingingForward)
            {
                swingingForward = false;
            }
            else
            {
                isSwinging = false;
                activeSwingMultiplier = 1f;
            }
        }
    }

    public float GetBatMultiplier() => isSwinging ? activeSwingMultiplier : 1f;

    private float GetCorrectedAngle(float angle)
    {
        return isFacingRight ? angle : -angle;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
        Vector2 center = (minBounds + maxBounds) / 2f;
        Vector2 size = new Vector2(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y);
        Gizmos.DrawWireCube(center, size);
    }
}