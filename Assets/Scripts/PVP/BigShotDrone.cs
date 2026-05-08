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
            if (bullet.CanIShoot()) { SpawnProjectile(shootAngle, shootPoint); }
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

    public override void SpawnProjectile(float angle, Transform pos)
    {
        // 1. Cooldown Check: Ensures the input system doesn't jam if multiple directions are hit
        if (Time.time < nextFireTime) return;
        if (projectilePrefab == null) return;

        nextFireTime = Time.time + fireRate;

        // 2. Spawn Logic
        Vector3 spawnPos = pos != null ? pos.position : transform.position;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        GameObject obj = Instantiate(projectilePrefab, spawnPos, rotation);
        obj.GetComponent<Projectile>().damage = bullet.currentDamage;
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
