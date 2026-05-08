using UnityEngine;
using UnityEngine.InputSystem;

public class TargetController : MonoBehaviour
{
    [Header("Target Movement")]
    [SerializeField] private float moveSpeed = 15f;

    [Header("Tether Settings")]
    [Tooltip("Drag the actual Drone GameObject here")]
    // 1. CHANGED: We now ask specifically for the DroneController script, not just the Transform
    [SerializeField] private PlayerController drone;

    [Tooltip("How far the target can get from the drone before it stops")]
    [SerializeField] private float maxRange = 8f;

    private Vector2 moveInput;

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // --- 2. NEW: The Target Box receives the inputs and passes them directly to the Drone ---

    public void OnShoot_Left(InputValue value)
    {
        if (drone != null) drone.OnShoot_Left(value);
    }

    public void OnShoot_Right(InputValue value)
    {
        if (drone != null) drone.OnShoot_Right(value);
    }

    public void OnShoot_Above(InputValue value)
    {
        if (drone != null) drone.OnShoot_Above(value);
    }

    public void OnShoot_Under(InputValue value)
    {
        if (drone != null) drone.OnShoot_Under(value);
    }

    public void OnFireLaser(InputValue value)
    {
        if (drone != null) drone.OnFireLaser(value);
    }

    // ----------------------------------------------------------------------------------------

    private void Update()
    {
        // Move the target normally
        transform.Translate(moveInput * moveSpeed * Time.deltaTime);

        // Enforce the maximum distance tether
        if (drone != null)
        {
            // Because 'drone' is now a script reference, we have to add .transform to get its position
            Vector2 offset = (Vector2)transform.position - (Vector2)drone.transform.position;

            if (offset.sqrMagnitude > maxRange * maxRange)
            {
                transform.position = (Vector2)drone.transform.position + Vector2.ClampMagnitude(offset, maxRange);
            }
        }
    }
}