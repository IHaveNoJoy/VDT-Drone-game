using UnityEngine;

[CreateAssetMenu(fileName = "ShootProjectileAttack", menuName = "Scriptable Objects/Boss Attacks/Bullet Pattern")]
public class ShootProjectileAttack : BossAttack
{
    [Header("Projectile Base Configuration")]
    public GameObject projectilePrefab;

    public int damageOverride = 0;
    public float speedOverride = 0f;

    public string targetTag = "Player";

    [Header("Pattern System")]
    public BossPattern[] possiblePatterns;

    public int bulletCount = 10;

    [Header("Random Spread (REAL 3D randomness)")]
    [Tooltip("Max cone angle in degrees")]
    public float spreadAngle = 60f;

    [Tooltip("0 = flat, 1 = full sphere randomness")]
    [Range(0f, 1f)]
    public float verticalRandomness = 0.5f;

    private static float spiralAngle;

    public override void Execute(BossController boss)
    {
        if (projectilePrefab == null)
            return;

        boss.Animator.SetTrigger(animationTrigger);
    }

    public override void OnImpact(BossController boss)
    {
        BossPattern pattern =
            possiblePatterns != null && possiblePatterns.Length > 0
                ? possiblePatterns[Random.Range(0, possiblePatterns.Length)]
                : BossPattern.Single;

        Transform t = boss.transform;

        switch (pattern)
        {
            case BossPattern.Single:
                SpawnSingle(t);
                break;

            case BossPattern.Radial:
                SpawnRadial(t);
                break;

            case BossPattern.Cone:
                SpawnCone(t);
                break;

            case BossPattern.Spiral:
                SpawnSpiral(t);
                break;
        }
    }

    // ---------------- CORE ----------------

    void SpawnBullet(Vector3 pos, Vector3 direction)
    {
        Quaternion rot = Quaternion.LookRotation(direction);

        GameObject instance = Object.Instantiate(projectilePrefab, pos, rot);

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

    // ---------------- TRUE RANDOM DIRECTION ----------------

    Vector3 GetRandomDirection(Transform origin)
    {
        // base forward direction
        Vector3 forward = origin.forward;

        // random horizontal rotation
        float yaw = Random.Range(-spreadAngle, spreadAngle);

        // random vertical deviation
        float pitch = Random.Range(-spreadAngle * verticalRandomness,
                                   spreadAngle * verticalRandomness);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);

        return rot * forward;
    }

    // ---------------- PATTERNS ----------------

    void SpawnSingle(Transform t)
    {
        SpawnBullet(t.position, GetRandomDirection(t));
    }

    void SpawnRadial(Transform t)
    {
        for (int i = 0; i < bulletCount; i++)
        {
            SpawnBullet(t.position, GetRandomDirection(t));
        }
    }

    void SpawnCone(Transform t)
    {
        Vector3 forward = t.forward;

        for (int i = 0; i < bulletCount; i++)
        {
            float yaw = Random.Range(-spreadAngle, spreadAngle);
            float pitch = Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f);

            Vector3 dir = Quaternion.Euler(pitch, yaw, 0) * forward;
            SpawnBullet(t.position, dir);
        }
    }

    void SpawnSpiral(Transform t)
    {
        Vector3 forward = t.forward;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = spiralAngle + (360f / bulletCount) * i;

            float pitch = Random.Range(-10f, 10f);

            Vector3 dir = Quaternion.Euler(pitch, angle, 0) * forward;

            SpawnBullet(t.position, dir);
        }

        spiralAngle += 15f;
    }
}