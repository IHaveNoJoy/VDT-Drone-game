using UnityEngine;

public abstract class BossAttack : ScriptableObject
{
    [Header("Base Attack Configurations")]
    public string attackName = "New Boss Attack";
    public int manaCost = 20;

    /// <param name="boss">A reference to the boss executing it, allowing access to its transform/stats.</param>
    public abstract void Execute(BossController boss);
}