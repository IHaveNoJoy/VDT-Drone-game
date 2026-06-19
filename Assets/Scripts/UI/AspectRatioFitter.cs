using UnityEngine;

[ExecuteAlways] // <-- This forces the script to run in the Editor without pressing Play!
[RequireComponent(typeof(Camera))]
public class AspectRatioFitter : MonoBehaviour
{
    [Header("Target Resolution")]
    public float targetWidth = 1920f;
    public float targetHeight = 1080f;

    // Choose the color of your gizmo box
    public Color gizmoColor = Color.green;

    // We changed this from Start() to Update() so it responds instantly to window resizing
    void Update()
    {
        ApplyAspectRatio();
    }

    void ApplyAspectRatio()
    {
        Camera cam = GetComponent<Camera>();

        // Calculate the aspect ratio dynamically based on your target resolution
        float targetAspect = targetWidth / targetHeight;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        // If the current window is wider than your target ratio (add pillarboxes)
        if (scaleHeight < 1.0f)
        {
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            cam.rect = rect;
        }
        // If the current window is taller than your target ratio (add letterboxes)
        else
        {
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            cam.rect = rect;
        }
    }

    // This method draws the visual guidelines in the Scene view
    void OnDrawGizmos()
    {
        Camera cam = GetComponent<Camera>();

        if (cam != null && cam.orthographic)
        {
            Gizmos.color = gizmoColor;

            float targetAspect = targetWidth / targetHeight;
            float height = cam.orthographicSize * 2f;
            float width = height * targetAspect;

            Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0.1f));
        }
    }
}