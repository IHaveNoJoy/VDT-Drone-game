using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BigShotDrone : PlayerController
{
    public InputActionReference shootInput;
    public float shootAngle = 0f;

    public BigShotBullet bullet;

    private bool canCharge = false;

    public override void Start()
    {
        base.Start();
        if (!isPlayer1) {shootAngle -= 180; }
    }
    public override void Update()
    {
        base.Update();

        if (isPlayer1) { canCharge = shootInput.action.IsPressed(); }
        else { canCharge = Input.GetKey(shoot); }


        if ((shootInput.action.WasReleasedThisFrame() && isPlayer1) ||
            (!isPlayer1 && Input.GetKeyUp(shoot)))
        {
            if (bullet.CanIShoot()) { SpawnProjectile(shootAngle); }
            bullet.ResetBullet();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (canCharge)
        {
            bullet.ChargeBullet();
        }
    }

    public override void SpawnProjectile(float angle)
    {
        // 1. Cooldown & Essential Null Checks
        if (Time.time < nextFireTime) return;
        if (projectilePrefab == null || bullet == null) return;

        nextFireTime = Time.time + fireRate;

        // 2. Spawn Logic (Now uses transform.position directly)
        Vector3 spawnPos = transform.position;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject obj = Instantiate(projectilePrefab, spawnPos, rotation);

        // 3. Component Setup
        Projectile proj = obj.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.damage = Convert.ToInt32(bullet.currentDamage);
        }

        // Apply scale based on bullet stats
        obj.transform.localScale *= bullet.currentSize;
    }

    public override void OnShoot_Right(InputValue value)
    {
        return;
    }
    public override void OnShoot_Above(InputValue value)
    {
        return;
    }
    public override void OnShoot_Left(InputValue value)
    {
        return;
    }
    public override void OnShoot_Under(InputValue value)
    {
        return;
    }
}
