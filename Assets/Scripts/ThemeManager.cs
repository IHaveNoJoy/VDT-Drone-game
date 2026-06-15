using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    [Header("References to Update")]
    public SpriteRenderer p1Paddle;
    public SpriteRenderer p2Paddle;
    public SpriteRenderer background;
    public SpriteRenderer ball;
    public TMP_Text victoryText;

    public GameTheme activeTheme;

    public void Start()
    {
        ApplyTheme(activeTheme);
    }

    public void SetTheme(GameTheme theme)
    {
        activeTheme = theme;
    }
    public void ApplyTheme(GameTheme newTheme)
    {
        activeTheme = newTheme;

        // Apply to objects that stay in the scene (Background, Paddles)
        background.sprite = activeTheme.backgroundSprite;
        p1Paddle.sprite = activeTheme.p1PaddleSprite;
        p2Paddle.sprite = activeTheme.p2PaddleSprite;

        // Refresh existing ball if it's already in the scene
        GameObject currentBall = GameObject.FindGameObjectWithTag("Ball");
        if (currentBall != null)
        {
            currentBall.GetComponent<SpriteRenderer>().sprite = activeTheme.ballSprite;
        }
    }

    // Call this specifically when the game ends
    public void UpdateVictoryText(bool p1Won)
    {
        if (activeTheme != null)
        {
            victoryText.text = p1Won ? activeTheme.victoryTextP1 : activeTheme.victoryTextP2;
        }
    }
}