using UnityEngine;
using UnityEngine.InputSystem;

public class TargetController : MonoBehaviour
{
    [Header("Target Movement")]
    [SerializeField] private float moveSpeed = 15f;

    [Header("Tether Settings")]
    [Tooltip("Drag the actual Drone (PlayerController) GameObject here")]
    [SerializeField] private PlayerController drone;

    [Tooltip("How far the target can get from the drone before it stops")]
    [SerializeField] private float maxRange = 8f;

    private Vector2 horizontalInput;
    private float heightInput;

    // --- RE-ROUTED INPUT CALLBACKS ---
    public void OnMove(InputValue value)
    {
        horizontalInput = value.Get<Vector2>();
        Debug.Log("I am moving: " + horizontalInput);
    }

    public void OnFlyHeight(InputValue value)
    {
        heightInput = value.Get<float>();
        Debug.Log("I am going up/down: " + heightInput);
    }

    public void OnShoot(InputValue value)
    {
        if (drone != null && value.isPressed)
        {
            drone.CommandFireLasers();
        }
    }

    public void OnShoot_Rockets(InputValue value)
    {
        if (drone != null && value.isPressed)
        {
            drone.CommandFireRockets();
        }
    }

    private void Update()
    {
        // 1. Calculate our intended movement vector in 3D space
        Vector3 movement = new Vector3(horizontalInput.x, heightInput, horizontalInput.y);

        // 2. Safely translate the target transform (No Rigidbody/Gravity needed)
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);

        // 3. Keep us within bounds of our flying drone
        if (drone != null && drone.rb != null)
        {
            Vector3 dronePos = drone.rb.position;
            Vector3 offset = transform.position - dronePos;

            if (offset.sqrMagnitude > maxRange * maxRange)
            {
                Vector3 clampedOffset = Vector3.ClampMagnitude(offset, maxRange);
                transform.position = dronePos + clampedOffset;
            }
        }
    }
}