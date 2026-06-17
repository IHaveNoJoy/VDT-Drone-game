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
    [Tooltip("Maximum charge level the player can reach.")]
    public int maxChargeLevel = 3;
    [Tooltip("Time in seconds to hold the button to reach the next charge level.")]
    public float chargeTimePerLevel = 0.5f;

    [Header("Visuals")]
    public GameObject chargeParticlePrefab;
    public Transform effectSpawnPoint;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isChargeButtonPressed;
    private bool isSwinging = false;
    private bool swingingForward = false;

    // Variables to hold the direction-corrected angles
    private float adjustedStartAngle;
    private float adjustedEndAngle;
    private float currentBatAngle = 0f;

    private bool isCharging = false;
    private float chargeTimer = 0f;

    // NEW: Integer-based charge tracking to match the BallController
    private int currentChargeLevel = 0;
    private int activeChargeLevel = 0;

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

        // Handle target cube
        if (targetCube != null) targetCube.SetParent(null);
        else
        {
            GameObject vt = new GameObject("VirtualTargetCube");
            targetCube = vt.transform;
            targetCube.position = transform.position;
        }
    }

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
            currentChargeLevel = 0; // A quick tap is a level 0 charge (normal speed)
        }

        if (isCharging && isChargeButtonPressed)
        {
            chargeTimer += Time.deltaTime;

            // Check if we hit the time threshold for the next level
            if (chargeTimer >= chargeTimePerLevel && currentChargeLevel < maxChargeLevel)
            {
                chargeTimer = 0f; // Reset timer for the next level
                currentChargeLevel++;

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

            // Lock in whatever level we reached so the ball can read it
            activeChargeLevel = currentChargeLevel;
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
                activeChargeLevel = 0; // Reset the active charge when the swing completely finishes
            }
        }
    }

    // NEW: Method called by BallController to get the integer charge level
    public int GetChargeLevel()
    {
        return isSwinging ? activeChargeLevel : 0;
    }

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