using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossController : GameStats
{
    [Header("Movement")]
    public float hoverSpeed = 2f;
    public float hoverAmount = 1f;
    private Vector3 startPos;

    [Header("Laser Battery")]
    public List<LaserController> laserBattery; // Drag all standard lasers here
    public float timeBetweenLasers = 0.5f;
    public float attackCooldown = 3.0f; // NEW: How long the boss waits before firing the sequence again

    [Header("Special Trap")]
    public LaserController specialLaser; // The one triggered by the collider
    public BoxCollider2D trapTrigger;    // The "Special Box Collider"

    public override void Start()
    {
        base.Start();
        startPos = transform.position;

        // NEW: Start the automatic attack loop when the boss spawns
        StartCoroutine(BossAttackLoop());
    }

    public override void Update()
    {
        base.Update();

        if (CurrentHP > 0)
        {
            HoverMovement();
        }
    }

    private void HoverMovement()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * hoverSpeed) * hoverAmount;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    // NEW: The continuous loop that makes the boss attack automatically
    private IEnumerator BossAttackLoop()
    {
        // Wait a brief moment before the first attack so the player can get ready
        yield return new WaitForSeconds(1f);

        while (CurrentHP > 0) // Keep looping as long as the boss is alive
        {
            yield return StartCoroutine(LaserSequenceRoutine()); // Wait for the firing sequence to finish
            yield return new WaitForSeconds(attackCooldown);     // Wait for the cooldown
        }
    }

    public void FireAllLasers()
    {
        StartCoroutine(LaserSequenceRoutine());
    }

    private IEnumerator LaserSequenceRoutine()
    {
        foreach (LaserController laser in laserBattery)
        {
            if (laser != null)
            {
                laser.FireLaser();
                yield return new WaitForSeconds(timeBetweenLasers);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (specialLaser != null)
            {
                Debug.Log("Boss: TRAP ACTIVATED!");
                specialLaser.FireLaser();
            }
        }
    }

    public override void Kill()
    {
        Debug.Log("Boss Defeated!");
        // Stop all coroutines so the boss stops shooting when it dies
        StopAllCoroutines();
        base.Kill();
    }

    public override void GetDamage(int Damage)
    {
        Debug.Log($"Boss took damage {Damage}");
        base.GetDamage(Damage);
    }
}