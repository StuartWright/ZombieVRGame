using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuWeaponSecondHandGrip : StuBaseGrabbable
{
    private BaseStuGun Weapon;
    public bool RotateBetweenHands;
    new void Awake()
    {
        base.Awake();
        Weapon = GetComponentInParent<BaseStuGun>();
        col.enabled = false;
        Weapon.GunGrabbed += GunGrabbed;
        Weapon.GunReleased += GunReleased;
    }

    public override void OnSelectEnter(StuGrabber interactor)
    {
        if(Weapon.CurrentInteractor != null)
        {
            /*
            Hand = interactor.GetComponentInChildren<HandPrecence>();
            Hand.SpawnedHandModel.SetActive(false);
            if (interactor.tag == "Right")
            {
                if (RightHand != null)
                    RightHand.SetActive(true);
            }
            else if (interactor.tag == "Left")
            {
                if (LeftHand != null)
                    LeftHand.SetActive(true);
            }
            */
            SetHandModel(interactor);
            Weapon.OnSecondHandGrab(interactor, RotateBetweenHands);
            //base.OnSelectEnter(interactor);
        }
        
    }
    public override void OnSelectExit(StuGrabber interactor)
    {
        /*
        Hand.SpawnedHandModel.SetActive(true);
        if (interactor.tag == "Right")
        {
            if (RightHand != null)
                RightHand.SetActive(false);
        }
        else if (interactor.tag == "Left")
        {
            if (LeftHand != null)
                LeftHand.SetActive(false);
        }
        */
        RemoveHandModel(interactor);
        Weapon.OnSecondHandRelease(interactor);
        //base.OnSelectExit(interactor);
    }
    public void GunGrabbed()
    {
        col.enabled = true;
    }
    public void GunReleased()
    {
        col.enabled = false;
    }
}
