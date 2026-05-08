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

    [Header("Combat")]
    public int contactDamage = 1;
    public float damageCooldown = 1f; // seconds between hits
}