using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public GameController gameController;
    public int playerID; // Set this to 1 for P1's goal, 2 for P2's goal

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            // If ball enters P1's goal, it means P2 scored!
            int scorer = (playerID == 1) ? 2 : 1;
            gameController.ScoreGoal(scorer);
        }
    }
}