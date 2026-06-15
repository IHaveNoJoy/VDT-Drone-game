using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "NewTheme", menuName = "Game/Theme")]
public class GameTheme : ScriptableObject
{
    public string ThemeName;
    public Sprite p1PaddleSprite;
    public Sprite p2PaddleSprite;
    public Sprite backgroundSprite;
    public Sprite ballSprite;
    public string victoryTextP1 = "Player 1 Wins!";
    public string victoryTextP2 = "Player 2 Wins!";
}