using Oculus.Interaction;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("UI & References")]
    public TMP_Text p1ScoreText;
    public TMP_Text p2ScoreText;
    public GameObject victoryScreen;
    public TMP_Text victoryText;
    public GameObject ballPrefab;
    public Transform ballSpawnPoint;
    public ThemeManager themeManager;

    [Header("Game Settings")]
    public int winningScore = 5;
    public float serveForce = 5f;

    private int scoreP1 = 0;
    private int scoreP2 = 0;
    private bool isGameOver = false;
    private int lastWinner = 0;
    private bool isGameRunning = false;

    void Start()
    {
        Time.timeScale = 1;

        if (victoryScreen != null)
        {
            victoryScreen.SetActive(false);
        }

        UpdateScoreUI();
    }

    // This is the "Brain" function called by the Goal triggers
    public void ScoreGoal(int playerWhoScored)
    {
        if (isGameOver || !isGameRunning) return;

        if (playerWhoScored == 1) scoreP1++;
        else scoreP2++;

        lastWinner = playerWhoScored; // The scorer becomes the next server
        UpdateScoreUI();

        if (scoreP1 >= winningScore || scoreP2 >= winningScore)
        {
            GameOver();
        }
        else
        {
            // Destroy existing ball and spawn new one
            GameObject existingBall = GameObject.FindGameObjectWithTag("Ball");
            if (existingBall != null) Destroy(existingBall);

            StartCoroutine(SpawnBallWithDelay());
        }
    }

    private void UpdateScoreUI()
    {
        p1ScoreText.text = scoreP1.ToString();
        p2ScoreText.text = scoreP2.ToString();
    }

    private IEnumerator SpawnBallWithDelay()
    {
        if (isGameRunning)
        {
            yield return new WaitForSeconds(1.5f);

            GameObject ball = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
            Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();

            // If lastWinner is 0 (start of game), pick random side
            // If lastWinner is 1, it means P1 scored, so the ball goes to P2 (Left)
            int directionMultiplier = 0;

            if (lastWinner == 0) directionMultiplier = (Random.value > 0.5f) ? 1 : -1;
            else directionMultiplier = (lastWinner == 1) ? -1 : 1; // P1 scored -> go left (-1), P2 scored -> go right (1)

            rb.linearVelocity = new Vector2(directionMultiplier * serveForce, 0);
        }
    }

    private void GameOver()
    {
        themeManager.UpdateVictoryText(scoreP1 > scoreP2);

        victoryScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void StartGame()
    {
        isGameRunning = true;
        Time.timeScale = 1;
        StartCoroutine(SpawnBallWithDelay());
        Debug.Log("Game Started!");
    }
    public void PauseGame()
    {
        isGameRunning = false;
        Time.timeScale = 0;
    }
}