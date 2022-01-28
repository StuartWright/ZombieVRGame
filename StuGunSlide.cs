using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class StuGunSlide : StuBaseGrabbable
{
    protected BaseStuGun Weapon;
    private BoxCollider Col;
    protected bool HasSlide;
    public float SlideForce, MaxSlideAmount;
    protected Vector3 StartPos;
    public override void Awake()
    {
        col = GetComponent<BoxCollider>();
        col.enabled = false;
        Weapon = GetComponentInParent<BaseStuGun>();
        Weapon.GunGrabbed += TurnColliderOn;
        Weapon.GunReleased += TurnColliderOff;
        Weapon.NoAmmo += SlideBack;
        StartPos = transform.localPosition;
        base.Awake();
    }
    protected float StartZ;
    protected float HandValue, HandValueToMinus;
    protected bool fullyback;
    public override void Interaction()
    {
        //HandValue = CurrentInteractor.transform.localPosition.z - HandValueToMinus;
        HandValue = Weapon.transform.InverseTransformPoint(CurrentInteractor.transform.position).z - HandValueToMinus;
        float value = StartZ + HandValue * SlideForce; 
        Vector3 pos = transform.localPosition;
        pos.z = Mathf.Clamp(value, MaxSlideAmount, StartPos.z);
        transform.localPosition = pos;
        if (transform.localPosition.z == MaxSlideAmount)
        {
            if (!fullyback)
            {
                fullyback = true;
                Weapon.Slide();
            }
        }
        else if(transform.localPosition.z == StartPos.z)
            fullyback = false;
    }
    protected void LateUpdate()
    {
        //if(!IsGrabbed && !HasSlide)
        if(!Weapon.ShootAnim.enabled && !HasSlide)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, StartPos, 0.1f);
            if (transform.localPosition.z >= StartPos.z -0.01f && !CurrentInteractor)
            {
                if (Weapon.ShootAnim != null)
                    Weapon.ShootAnim.enabled = true;
                Weapon.CanFire = true;
            }
        }
    }
    public override void OnSelectEnter(StuGrabber interactor)
    {
        if (Weapon.CurrentInteractor == null) return;
        StartZ = transform.localPosition.z;
        if(Weapon.ShootAnim != null)
            Weapon.ShootAnim.enabled = false;
        if(!HasSlide)
        Weapon.CanFire = false;
        col.enabled = false;
        TargetDevice = interactor.TargetDevice;
        CurrentInteractor = interactor;
        //HandValueToMinus = CurrentInteractor.transform.localPosition.z;
        HandValueToMinus = Weapon.transform.InverseTransformPoint(CurrentInteractor.transform.position).z;
        if (HasSlide)
        {
            if(Weapon.Slide())
            {
                HasSlide = false;
                IsGrabbed = true;
                fullyback = true;
                Weapon.CanFire = false;
            }
        }
        else
            IsGrabbed = true;
        SetHandModel(interactor);
    }
    public void ManualCockGun()
    {
        HasSlide = false;
        fullyback = true;
    }
    public override void OnSelectExit(StuGrabber interactor)
    {
        RemoveHandModel(interactor);
        col.enabled = true;
        CurrentInteractor = null;
        IsGrabbed = false;
    }
    public virtual void SlideBack()
    {
        if(!HasSlide)
        {
            if (Weapon.ShootAnim != null)
                Weapon.ShootAnim.enabled = false;
            transform.localPosition = StartPos;
            HasSlide = true;
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z - Mathf.Abs(StartPos.z - MaxSlideAmount) * 0.7f);
        }        
    }
    private void TurnColliderOn()
    {
        col.enabled = true;
    }
    private void TurnColliderOff()
    {
        col.enabled = false;
    }
}
