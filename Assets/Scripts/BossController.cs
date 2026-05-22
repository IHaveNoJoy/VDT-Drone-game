using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossController : GameStats
{
    [Header("Predictable Arena Bounds (3D Cube)")]
    private Vector3 zoneCenter;
    public Vector3 zoneSize = new Vector3(10f, 5f, 5f);
    public float movementSpeed = 3f;
    public float timeSpentAtPosition = 1.5f;

    [Header("Mana Economy")]
    public float maxMana = 100f;
    public float currentMana = 30f;
    public float manaRegenRate = 5f;

    [Header("AI Logic & Cooldowns")]
    [Tooltip("How long the boss rests/thinks after executing ANY attack.")]
    public float globalAttackCooldown = 2.5f;

    [Tooltip("Drag your ScriptableObject attack assets directly into this list!")]
    public List<BossAttack> attackPool;

    private Vector3 targetPosition;
    private bool isMovingToPosition = false;

    public override void Start()
    {
        base.Start();
        zoneCenter = transform.position;
        StartCoroutine(MovementLoop());
        StartCoroutine(AIBrainLoop());
    }

    public override void Update()
    {
        base.Update();

        if (CurrentHP > 0)
        {
            RegenerateMana();
        }
    }

    private void RegenerateMana()
    {
        if (currentMana < maxMana)
        {
            currentMana += manaRegenRate * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, maxMana);
        }
    }

    #region Predictable Movement Logic

    private IEnumerator MovementLoop()
    {
        targetPosition = GetRandomPointInZone();

        while (CurrentHP > 0)
        {
            isMovingToPosition = true;

            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
                yield return null;
            }

            isMovingToPosition = false;
            yield return new WaitForSeconds(timeSpentAtPosition);
            targetPosition = GetRandomPointInZone();
        }
    }

    private Vector3 GetRandomPointInZone()
    {
        Vector3 minBounds = zoneCenter - (zoneSize / 2f);
        Vector3 maxBounds = zoneCenter + (zoneSize / 2f);

        return new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y),
            Random.Range(minBounds.z, maxBounds.z)
        );
    }

    #endregion

    #region Intelligent Tactical Loop

    private IEnumerator AIBrainLoop()
    {
        yield return new WaitForSeconds(1.5f);

        while (CurrentHP > 0)
        {
            List<BossAttack> affordableAttacks = new List<BossAttack>();

            // Filter attacks based on current mana availability
            foreach (BossAttack attack in attackPool)
            {
                if (attack != null && attack.manaCost <= currentMana)
                {
                    affordableAttacks.Add(attack);
                }
            }

            if (affordableAttacks.Count > 0)
            {
                // Select an affordable attack asset at random
                BossAttack chosenAttack = affordableAttacks[Random.Range(0, affordableAttacks.Count)];

                currentMana -= chosenAttack.manaCost;

                // Pass 'this' boss instance context into the attack execution
                chosenAttack.Execute(this);

                yield return new WaitForSeconds(globalAttackCooldown);
            }
            else
            {
                // Wait briefly before re-checking mana resources
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    #endregion

    public override void Kill()
    {
        Debug.Log("Boss Defeated!");
        StopAllCoroutines();
        base.Kill();
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
        {
            zoneCenter = transform.position;
        }
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(zoneCenter, zoneSize);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawSphere(targetPosition, 0.4f);
        }
    }
}