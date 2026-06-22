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

    [Header("Play Area Boundary")]
    [Tooltip("Center of the Gizmo box in the scene")]
    public Vector2 playAreaCenter = Vector2.zero;
    [Tooltip("Width and Height of the Gizmo box")]
    public Vector2 playAreaSize = new Vector2(20f, 15f);
    public Color gizmoColor = Color.yellow;

    private int scoreP1 = 0;
    private int scoreP2 = 0;
    private bool isGameOver = false;
    private int lastWinner = 0;
    private bool isGameRunning = false;

    // Cache the active ball to avoid heavy Find calls in Update
    private GameObject currentBall;

    void Start()
    {
        Time.timeScale = 1;

        if (victoryScreen != null)
        {
            victoryScreen.SetActive(false);
        }

        UpdateScoreUI();
    }

    void Update()
    {
        // Continuously check if the ball is inside the play area bounds
        if (isGameRunning && currentBall != null)
        {
            // Create a bounds object (using a large Z value just in case of 2D depth discrepancies)
            Bounds playArea = new Bounds(playAreaCenter, new Vector3(playAreaSize.x, playAreaSize.y, 100f));

            // If the ball leaves the box, destroy and respawn it
            if (!playArea.Contains(currentBall.transform.position))
            {
                Debug.LogWarning("Ball escaped the play area bounds. Respawning...");
                Destroy(currentBall);
                StartCoroutine(SpawnBallWithDelay());
            }
        }
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
            if (currentBall != null) Destroy(currentBall);

            // Backup catch just in case something else spawned a ball
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
            yield return new WaitForSeconds(3f);

            // Assign the newly spawned ball to our cached variable
            currentBall = Instantiate(ballPrefab, ballSpawnPoint.position, Quaternion.identity);
            Rigidbody2D rb = currentBall.GetComponent<Rigidbody2D>();

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

    // Draws the boundary box in the Unity Editor Scene View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(playAreaCenter, new Vector3(playAreaSize.x, playAreaSize.y, 0.1f));
    }
}