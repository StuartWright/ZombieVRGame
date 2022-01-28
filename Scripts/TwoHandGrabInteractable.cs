using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class TwoHandGrabInteractable : XRGrabInteractable
{
    public List<XRSimpleInteractable> SecondHandgrabPoints = new List<XRSimpleInteractable>();
    private XRBaseInteractor SecondInteractor;
    private Quaternion AttachInitialRotation;
    public Transform MainGrabPoint;
    public enum TwoHandRotationType {None, First, Second };
    public TwoHandRotationType RotationType;
    public bool SnapToSecondHand = true;
    private Quaternion InitialRotationOffset;
    private void Start()
    {
        foreach (XRSimpleInteractable item in SecondHandgrabPoints)
        {
            item.onSelectEntered.AddListener(OnSecondHandGrab);
            item.onSelectExited.AddListener(OnSecondHandRelease);
        }
    }
    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        print("first grab enter");
        base.OnSelectEntered(interactor);
        AttachInitialRotation = interactor.attachTransform.localRotation;
    }
    protected override void OnSelectExited(XRBaseInteractor interactor)
    {
        print("first grab exit");
        base.OnSelectExited(interactor);
        SecondInteractor = null;
        interactor.attachTransform.localRotation = AttachInitialRotation;
    }
    public void OnSecondHandGrab(XRBaseInteractor interactor)
    {
        
        print("second grab enter");
        SecondInteractor = interactor;
        InitialRotationOffset = Quaternion.Inverse(GetTwoHandRotation()) * CurrentInteractor.attachTransform.rotation;
    }
    public void OnSecondHandRelease(XRBaseInteractor interactor)
    {
        print("second grab exit");
        SecondInteractor = null;
    }

    public Quaternion GetTwoHandRotation()
    {
        Quaternion targetRotation;
        if (RotationType == TwoHandRotationType.None)
        {
            targetRotation = Quaternion.LookRotation(SecondInteractor.attachTransform.position - CurrentInteractor.attachTransform.position);
        }
        else if(RotationType == TwoHandRotationType.First)
        {
            targetRotation = Quaternion.LookRotation(SecondInteractor.attachTransform.position - CurrentInteractor.attachTransform.position, CurrentInteractor.transform.up);
        }
        else
        {
            targetRotation = Quaternion.LookRotation(SecondInteractor.attachTransform.position - CurrentInteractor.attachTransform.position, SecondInteractor.transform.up);
        }
        return targetRotation;
    }
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if(SecondInteractor && CurrentInteractor)
        {
            if(SnapToSecondHand)
                CurrentInteractor.attachTransform.rotation = GetTwoHandRotation();
            else
                CurrentInteractor.attachTransform.rotation = GetTwoHandRotation() * InitialRotationOffset;
        }
        base.ProcessInteractable(updatePhase);
    }
    private XRBaseInteractor CurrentInteractor;
    public override bool IsSelectableBy(XRBaseInteractor interactor)
    {
        if(!isSelected)
        {
            CurrentInteractor = interactor;
        }
        bool iscurrent = interactor.Equals(CurrentInteractor);
        if (!iscurrent)
            print("gsr");
        return base.IsSelectableBy(interactor) && iscurrent;
    }
}
