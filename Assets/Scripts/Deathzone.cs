using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.TryGetComponent<GameStats>(out GameStats stats))
        {
            if (stats != null)
            {
                Debug.Log($"Safety Check Passed: Killing {collision.name}");
                stats.Kill();
            }
        }
    }
}