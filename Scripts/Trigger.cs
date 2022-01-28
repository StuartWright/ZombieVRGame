using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public bool CanFire, Automatic, TriggerDown;
    public BaseVRGun gun;
    public void SetFalse()
    {
        CanFire = false;
    }
    public void SetTrue()
    {
        CanFire = true;
        if (Automatic && TriggerDown)
            Shoot();
    }
    public void Shoot()
    {
        TriggerDown = true;
        if(CanFire)
        gun.FireAnim();
    }
    public void Fire()
    {
        gun.Fire();
    }
    public void TriggerUp()
    {
        TriggerDown = false;
    }
}
