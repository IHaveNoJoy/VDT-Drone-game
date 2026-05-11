using UnityEngine;
using UnityEngine.InputSystem;

public class BasicDrone : PlayerController
{
    public InputActionReference shootInput;
    public float shootAngle = 0f;

    public InputActionReference angleUp;
    public InputActionReference angleDown;
    public float shootAngleChangeSpeed = 0.3f;

    public Transform shootPointObj;

    public KeyCode aimUp;
    public KeyCode aimDown;
    

    public override void Update()
    {
        base.Update();


        if ((shootInput.action.IsPressed() && isPlayer1) || (!isPlayer1 && Input.GetKeyDown(shoot)))
        {
            int newAngle = 0;
            if (!isPlayer1) { newAngle = -180; }
            SpawnProjectile(shootAngle + newAngle);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if ((angleUp.action.IsPressed() && isPlayer1) || (!isPlayer1 && Input.GetKey(aimUp)))
        {
            shootAngle += shootAngleChangeSpeed;
        }

        if ((angleUp.action.IsPressed() && isPlayer1) || (!isPlayer1 && Input.GetKey(aimDown)))
        {
            shootAngle -= shootAngleChangeSpeed;
        }



        Quaternion rotation = Quaternion.Euler(shootPoint.rotation.x, shootPoint.rotation.y, shootAngle);
        shootPointObj.rotation = rotation;
        shootPointObj.localRotation *= Quaternion.Euler(0, 0, 180);
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
