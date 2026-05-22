using UnityEngine;
using System.Collections;

public class ARBossController : GameStats
{
    [Header("Hover")]
    public float hoverSpeed = 2f;
    public float hoverAmount = 0.3f;

    [Header("Arena Movement")]
    public float arenaMoveSpeed = 1f;
    public float arenaRange = 2f;

    [Header("Attack")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float timeBetweenShots = 0.5f;
    public int shotsPerBurst = 3;
    public float attackCooldown = 3f;

    [Header("Target")]
    public Transform target;

    private Vector3 startPos;
    private Vector3 moveTarget;

    public override void Start()
    {
        base.Start();

        startPos = transform.position;
        PickNewMoveTarget();

        if (target == null && Camera.main != null)
        {
            target = Camera.main.transform;
        }

        StartCoroutine(BossAttackLoop());
    }

    public override void Update()
    {
        base.Update();

        if (CurrentHP <= 0) return;

        HoverMovement();
        ArenaMovement();
        FaceTarget();
    }

    private void HoverMovement()
    {
        float newY =
            startPos.y +
            Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;

        transform.position = new Vector3(
            transform.position.x,
            newY,
            transform.position.z
        );
    }

    private void ArenaMovement()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            moveTarget,
            arenaMoveSpeed * Time.deltaTime
        );

        float distance =
            Vector3.Distance(transform.position, moveTarget);

        if (distance < 0.2f)
        {
            PickNewMoveTarget();
        }
    }

    private void PickNewMoveTarget()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-arenaRange, arenaRange),
            0,
            Random.Range(-arenaRange, arenaRange)
        );

        moveTarget = startPos + randomOffset;
    }

    private void FaceTarget()
    {
        if (target == null) return;

        Vector3 lookDirection =
            target.position - transform.position;

        lookDirection.y = 0;

        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(lookDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 2f
            );
        }
    }

    private IEnumerator BossAttackLoop()
    {
        yield return new WaitForSeconds(1f);

        while (CurrentHP > 0)
        {
            yield return StartCoroutine(ProjectileBurstRoutine());

            yield return new WaitForSeconds(attackCooldown);
        }
    }

    private IEnumerator ProjectileBurstRoutine()
    {
        for (int i = 0; i < shotsPerBurst; i++)
        {
            FireProjectile();

            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    private void FireProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
            return;

        Vector3 direction =
            (target.position - firePoint.position).normalized;

        Quaternion rotation =
            Quaternion.LookRotation(direction);

        Instantiate(
            projectilePrefab,
            firePoint.position,
            rotation
        );
    }

    public override void Kill()
    {
        Debug.Log("AR Boss Defeated!");

        StopAllCoroutines();

        base.Kill();
    }
}