using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChargerDrone : PlayerController
{
    public InputActionReference shootInput;
    public float shootAngle = 0f;

    public List<ChargerDrone_Bullet> shootPoints;
    public float shootSpeed = 0.2f;

    public override void Update()
    {
        base.Update();

        ChargeBullets();

        if ((shootInput.action.IsPressed() && isPlayer1) || (!isPlayer1 && Input.GetKeyDown(shoot)))
        {
            ShootReadyBullets();
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void ChargeBullets()
    {
        foreach (ChargerDrone_Bullet bullet in shootPoints)
        {
            if (bullet.CanIBeShot())
            {
                continue;
            }
            else
            {
                bullet.Charge();
                break;
            }
        }
    }

    public void ShootReadyBullets()
    {
        StartCoroutine(ShootBullets(shootPoints));      
    }

    IEnumerator ShootBullets(List<ChargerDrone_Bullet> bullets)
    {
        for (int i = 0; i < bullets.Count; i++)
        {
            yield return new WaitForSeconds(shootSpeed);
            if (bullets[i].CanIBeShot()){
                SpawnProjectile(bullets[i].angle);
                bullets[i].ResetCharge();
            }
        }

    }

    public override void SpawnProjectile(float angle)
    {
        // 1. Simplified Spawn Logic: Defaults directly to the object's current position
        Vector3 spawnPos = transform.position;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        // 2. Instantiate the projectile
        if (projectilePrefab != null)
        {
            Instantiate(projectilePrefab, spawnPos, rotation);
        }
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
