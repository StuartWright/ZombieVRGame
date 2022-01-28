using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class GunMagazine : XRSocketInteractor
{
    //public BaseVRGun gun;
    public string TargetTag;
    public override bool CanSelect(XRBaseInteractable interactable)
    {
       // if (GameManager.Instance.MagRelease)
         //   return false;
        //else 
            return base.CanSelect(interactable) && interactable.CompareTag(TargetTag);

    }
    protected override void OnSelectExiting(XRBaseInteractable interactable)
    {
        base.OnSelectExiting(interactable);
    }
    protected override void OnSelectExited(XRBaseInteractable interactable)
    {
        base.OnSelectExited(interactable);
    }
    
    
    /*
    XRBaseInteractable meg;
    public void AddMag(XRBaseInteractable mag)
    {
        //selectTarget = mag;
        OnSelectEntering(mag);
        OnSelectEntered(mag);
        print(selectTarget);
    }
    protected override void OnSelectEntering(XRBaseInteractable interactable)
    {
        base.OnSelectEntering(interactable);
    }
    protected override void OnSelectEntered(XRBaseInteractable interactable)
    {
        base.OnSelectEntered(interactable);
    }
    */
}
