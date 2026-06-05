using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bullet;
    public float RateOfFire;

    private bool shooting = false;
    private float t = 0;
    private float triggerInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        triggerInput = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);

        if(triggerInput > 0)
        {
            shooting = true;
        }
        else
        {
            shooting = false;
        }

        ManageRateOfFire();
    }

    public void Shoot()
    {
        Instantiate(bullet, transform.position, transform.rotation);
    }

    public void ManageRateOfFire()
    {
        if(t <= 0 && shooting)
        {
            Shoot(); 
            t = RateOfFire;
        }

        if(t >= 0)
        {
            t = t - Time.deltaTime;
        }
    }
}
