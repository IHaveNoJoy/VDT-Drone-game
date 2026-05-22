using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class ControlTest1 : MonoBehaviour
{
    public GameObject obj;
    public Vector2 moveVector;
    public Transform t;
    public float moveY;
    public float movementSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        //Thumbstick
        moveVector = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        //Move Y-axis
        //Button A/X
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            moveY = -1;
        }
        //Button B/Y
        else if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            moveY = 1;
        }

        //Reset Y to 0 on release
        //Button A/X
        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            moveY = 0;
        }
        //Button B/Y
        if (OVRInput.GetUp(OVRInput.Button.Two))
        {
            moveY = 0;
        }

        //Move X/Z-axis
        Vector3 movement = new Vector3(moveVector.x, moveY, moveVector.y);
        movement.Normalize();

        //Move
        t.Translate(movementSpeed * movement * Time.deltaTime);
    }
}
