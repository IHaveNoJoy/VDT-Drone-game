using UnityEngine;

[CreateAssetMenu(fileName = "ShootProjectileAttack", menuName = "Scriptable Objects/Boss Attacks/Shoot Projectile")]
public class ShootProjectileAttack : BossAttack
{
    [Header("Projectile Base Configuration")]
    public GameObject projectilePrefab;

    public int damageOverride = 0;
    public float speedOverride = 0f;

    public string targetTag = "Player";

    public override void Execute(BossController boss)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning($"[{attackName}] Missing projectile prefab!");
            return;
        }

        boss.Animator.SetTrigger(animationTrigger);
    }

    public override void OnImpact(BossController boss)
    {
        GameObject instance = Object.Instantiate(
            projectilePrefab,
            boss.transform.position,
            boss.transform.rotation
        );

        if (instance.TryGetComponent<Projectile>(out Projectile proj))
        {
            if (damageOverride > 0) proj.damage = damageOverride;
            if (speedOverride > 0f) proj.speed = speedOverride;

            if (proj is GravitationalBullet gb)
            {
                gb.targetTag = targetTag;
            }
        }
    }
}