using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponHolsterTypes
{
    SideArm,
    MainArm
}
public class StuWeaponHolster : StuBaseSocket
{
    public BaseStuGun Weapon;
    public Vector3 Rotation;
    public WeaponHolsterTypes HolsterType;
    private void LateUpdate()
    {
        //if (HasObject) return;
        if(Weapon == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 0.2f);
            foreach (Collider col in colliders)
            {
                if (col.GetComponent<BaseStuGun>())
                {                    
                    Weapon = col.GetComponent<BaseStuGun>();
                    if (Weapon.HolsterType == HolsterType)
                    {
                        OnSelectEnter(Weapon);
                        break;
                    }
                    else
                        Weapon = null;
                }
            }
        }  
        else
        {
            if(!Weapon.IsGrabbed)
            {
                Vector3 Offset = Weapon.transform.position - Weapon.HolsterOffsetPoint.position;
                Weapon.transform.position = transform.position + Offset;                
            }           
        }
    }
    public override void OnSelectEnter(StuBaseGrabbable interactor)
    {
        HasObject = true;
        interactor.IsInHolster = true;
        interactor.Holster = this;
        //WeaponRot = Weapon.HolsterOffsetPoint.localEulerAngles;       
        Weapon.transform.SetParent(transform);       
        Weapon.transform.localEulerAngles = Rotation;
        interactor.InHolster();
        base.OnSelectEnter(interactor);
    }
    public override void OnSelectExit(StuBaseGrabbable interactor)
    {
        interactor.IsInHolster = false;
        interactor.Holster = null;
        interactor.OutOfHolster();
        Weapon = null;
        base.OnSelectExit(interactor);
    }

    protected override void CheckExit(Collider other)
    {
       
    }
}
