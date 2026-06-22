using UnityEngine;

public abstract class BossAttack : ScriptableObject
{
    public enum BossPattern
{
    Single,
    Radial,
    Cone,
    Spiral
}
    [Header("Base Attack Configurations")]
    public string attackName = "New Boss Attack";
    public int manaCost = 20;

    [Header("Animation")]
    public string animationTrigger;

    // START attack (animation begins)
    public abstract void Execute(BossController boss);

    // IMPACT moment (Animation Event triggers this)
    public virtual void OnImpact(BossController boss) { }
}