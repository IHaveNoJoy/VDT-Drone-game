using UnityEngine;

[CreateAssetMenu(fileName = "ShootProjectileAttack", menuName = "Scriptable Objects/Boss Attacks/Shoot Projectile")]
public class ShootProjectileAttack : BossAttack
{
    [Header("Projectile Base Configuration")]
    [Tooltip("Drag ANY prefab here that has a script inheriting from the 'Projectile' parent class.")]
    public GameObject projectilePrefab;

    [Tooltip("Override the projectile's base damage. Set to 0 or less to keep the prefab's default damage value.")]
    public int damageOverride = 0;

    [Tooltip("Override the projectile's speed. Set to 0 or less to keep the prefab's default speed.")]
    public float speedOverride = 0f;

    [Header("Targeting Settings (If applicable)")]
    [Tooltip("If the projectile is a tracking bullet (like GravitationalBullet), it will look for this tag.")]
    public string targetTag = "Player";

    public override void Execute(BossController boss)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning($"[BOSS AI] {attackName} failed to fire because no Projectile prefab is assigned!");
            return;
        }

        // 1. Temporarily deactivate the root prefab layout if it uses immediate finding logic in Start()
        bool originalActiveState = projectilePrefab.activeSelf;
        projectilePrefab.SetActive(false);

        // 2. Instantiate the projectile at the boss position/rotation
        GameObject projectileInstance = Instantiate(projectilePrefab, boss.transform.position, boss.transform.rotation);

        // 3. Fetch the parent component class 'Projectile'
        if (projectileInstance.TryGetComponent<Projectile>(out Projectile baseProjectile))
        {
            // Inject overrides if you configured them in the script asset
            if (damageOverride > 0) baseProjectile.damage = damageOverride;
            if (speedOverride > 0f) baseProjectile.speed = speedOverride;

            // 4. Use Polymorphism: Check if this specific projectile happens to be a GravitationalBullet
            if (baseProjectile is GravitationalBullet gravityBullet)
            {
                gravityBullet.targetTag = targetTag;
                Debug.Log($"[BOSS AI] Fired {attackName}: Detected specialized GravitationalBullet logic. Assigned target: '{targetTag}'.");
            }
            else
            {
                Debug.Log($"[BOSS AI] Fired {attackName}: Spawning standard base Projectile style layout.");
            }
        }

        // 5. Wake up the instance and restore prefab template integrity
        projectileInstance.SetActive(true);
        projectilePrefab.SetActive(originalActiveState);
    }
}