using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public GameController gameController;
    public int playerID; // Set this to 1 for P1's goal, 2 for P2's goal

    [Header("Visual Effects")]
    public GameObject explosionPrefab; // Drag the correct P1 or P2 prefab here in the Inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            if (explosionPrefab != null)
            {
                Vector3 spawnPos = collision.transform.position; // Default fallback position
                Quaternion spawnRot = Quaternion.identity;       // Default fallback rotation

                // Apply the exact transform for P1's goal
                if (playerID == 1)
                {
                    spawnPos = new Vector3(-10.09f, -0.43f, 0f);
                    spawnRot = Quaternion.Euler(0f, 0f, -87.704f);
                }
                // Apply the exact transform for P2's goal (from image_879cfe.png)
                else if (playerID == 2)
                {
                    spawnPos = new Vector3(10.12f, -0.43f, 0f);
                    spawnRot = Quaternion.Euler(0f, 0f, 87f);
                }

                Instantiate(explosionPrefab, spawnPos, spawnRot);
            }

            // If ball enters P1's goal, it means P2 scored (and vice versa)!
            int scorer = (playerID == 1) ? 2 : 1;
            gameController.ScoreGoal(scorer);
        }
    }
}