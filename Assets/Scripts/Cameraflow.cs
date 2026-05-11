using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Drag the object you want to follow here (e.g., your Drone)")]
    [SerializeField] private Transform target;

    [Tooltip("How far the camera sits away. Leave Z at -10 for 2D!")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Camera Feel")]
    [Tooltip("Approximate time it takes to reach the target. Higher = more floaty/delayed.")]
    [SerializeField] private float smoothTime = 0.25f;

    private Vector3 currentVelocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, smoothTime);
    }
}