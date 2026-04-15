using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Identity")]
    public string enemyName;

    [Header("Visual")]
    public Sprite sprite;

    [Header("Stats")]
    public int maxHp;
    public float moveSpeed;
    public float size;
}