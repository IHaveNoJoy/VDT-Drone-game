using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossController : GameStats
{
    private Animator animator;
    public Animator Animator => animator;

    private BossAttack currentAttack;

    [Header("Arena")]
    private Vector3 zoneCenter;
    public Vector3 zoneSize = new Vector3(10f, 5f, 5f);
    public float movementSpeed = 3f;
    public float timeSpentAtPosition = 1.5f;

    [Header("Mana")]
    public float maxMana = 100f;
    public float currentMana = 30f;
    public float manaRegenRate = 5f;

    [Header("AI")]
    public float globalAttackCooldown = 2.5f;
    public List<BossAttack> attackPool;

    private Vector3 targetPosition;

    public override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
        zoneCenter = transform.position;

        StartCoroutine(MovementLoop());
        StartCoroutine(AIBrainLoop());
    }

    public override void Update()
    {
        base.Update();

        if (CurrentHP > 0)
            RegenerateMana();
    }

    private void RegenerateMana()
    {
        if (currentMana < maxMana)
        {
            currentMana += manaRegenRate * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, maxMana);
        }
    }

    #region AI

    private IEnumerator AIBrainLoop()
    {
        yield return new WaitForSeconds(1.5f);

        while (CurrentHP > 0)
        {
            List<BossAttack> valid = new List<BossAttack>();

            foreach (var attack in attackPool)
            {
                if (attack != null && attack.manaCost <= currentMana)
                    valid.Add(attack);
            }

            if (valid.Count == 0)
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }

            BossAttack chosen =
                valid[Random.Range(0, valid.Count)];

            currentMana -= chosen.manaCost;

            currentAttack = chosen;

            // START ATTACK (animation only)
            currentAttack.Execute(this);

            yield return new WaitForSeconds(globalAttackCooldown);
        }
    }

    #endregion

    #region Animation Event Bridge

    // Called from Animation Event at correct frame
    public void Animation_Impact()
    {
        currentAttack?.OnImpact(this);
    }

    #endregion

    #region Movement (unchanged)

    private IEnumerator MovementLoop()
    {
        targetPosition = GetRandomPointInZone();

        while (CurrentHP > 0)
        {
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition,
                    movementSpeed * Time.deltaTime
                );

                yield return null;
            }

            yield return new WaitForSeconds(timeSpentAtPosition);
            targetPosition = GetRandomPointInZone();
        }
    }

    private Vector3 GetRandomPointInZone()
    {
        Vector3 min = zoneCenter - zoneSize / 2f;
        Vector3 max = zoneCenter + zoneSize / 2f;

        return new Vector3(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            Random.Range(min.z, max.z)
        );
    }

    #endregion
}