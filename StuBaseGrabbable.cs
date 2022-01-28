using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public enum InteractionButtons
{
    Grip,
    Trigger,
    AButton
}
public enum HandAnimation
{
    HandTrigger,
    Fist,
    BarrelGrip1,
    BarrelGrip2,
    SlideGrip1,
    SlideGrip2,
    HandRelaxedFist,
    CapTouch,
    Pinch,
    M4A1Slide
}
public class StuBaseGrabbable : MonoBehaviour
{
    public InteractionButtons ButtonToInteract;
    public Transform GripPoint, RightHandModelGripPoint, LeftHandModelGripPoint;
    [HideInInspector]
    public bool IsGrabbed;
    [HideInInspector]
    public StuGrabber CurrentInteractor, SecondInteractor;
    protected InputDevice TargetDevice;
    protected Rigidbody RB;
    protected Collider col;
    public HandAnimation HandAnim;
    [HideInInspector]
    public bool IsInHolster;
    [HideInInspector]
    public StuWeaponHolster Holster;
    public virtual void Awake()
    {
        RB = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public virtual void OnSelectEnter(StuGrabber interactor)
    {
        col.enabled = false;
        TargetDevice = interactor.TargetDevice;
        CurrentInteractor = interactor;
        IsGrabbed = true;
        SetHandModel(interactor);
        transform.SetParent(interactor.transform);
        if(RB != null)
        RB.isKinematic = true;
    }
    public virtual void OnSelectExit(StuGrabber interactor)
    {
        RemoveHandModel(interactor);
        col.enabled = true;
        CurrentInteractor = null;
        IsGrabbed = false;
        transform.SetParent(null);
        if(RB != null)
        RB.isKinematic = false;
        ThrowObject(interactor);
    }
    private bool TriggerPressed, DeactivateOnce = false, SecondaryPressed, DeactivateSecondaryOnce = false;
    private Vector3 lastPosition, LastAnglePosition;
    private Vector3 AngleVelocity = Vector3.zero;
    public Vector3 velocity = Vector3.zero;
    public virtual void Update()
    {        
        if (IsGrabbed)
        {
            velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;
            //AngleVelocity = (transform.eulerAngles - LastAnglePosition) / Time.deltaTime;
            //LastAnglePosition = transform.eulerAngles;
            Interaction();
        }            
    }

    public virtual void Interaction()
    {
        AttachToHand();
        if (TargetDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool TriggerDown) && TriggerDown && CurrentInteractor != null && !TriggerPressed)
        {
            DeactivateOnce = true;
            TriggerPressed = true;
            OnActivate();
        }
        else if (!TriggerDown && DeactivateOnce)
        {
            DeactivateOnce = false;
            TriggerPressed = false;
            OnDeactivate();
        }
        if (TargetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool SecondaryDown) && SecondaryDown && CurrentInteractor != null && !SecondaryPressed)
        {
            DeactivateSecondaryOnce = true;
            SecondaryPressed = true;
            OnSecondaryButtonActivate();
        }
        else if(!SecondaryDown && DeactivateSecondaryOnce)
        {
            DeactivateSecondaryOnce = false;
            SecondaryPressed = false;
            OnSecondaryButtonDeactivate();
        }
    }
    protected virtual void AttachToHand()
    {
        if (GripPoint != null)
        {
            Vector3 Offset = transform.position - GripPoint.position;
            transform.position = CurrentInteractor.transform.position + Offset;
            if (!SecondInteractor)
                transform.localRotation = GripPoint.localRotation;
        }
        else
        {
            transform.position = CurrentInteractor.transform.position;
            if (!SecondInteractor)
                transform.localRotation = transform.localRotation;
        }
    }
    private HandPrecence Hand;
    public virtual void SetHandModel(StuGrabber interactor)
    {
        if(RightHandModelGripPoint != null && LeftHandModelGripPoint != null)
        {
            if (interactor.tag == "Right")
            {
                interactor.Hand.SpawnedHandModel.transform.position = RightHandModelGripPoint.position;
                interactor.Hand.SpawnedHandModel.transform.rotation = RightHandModelGripPoint.rotation;
                interactor.Hand.SpawnedHandModel.transform.SetParent(RightHandModelGripPoint.transform);
            }
            else if (interactor.tag == "Left")
            {
                interactor.Hand.SpawnedHandModel.transform.position = LeftHandModelGripPoint.position;
                interactor.Hand.SpawnedHandModel.transform.rotation = LeftHandModelGripPoint.rotation;
                interactor.Hand.SpawnedHandModel.transform.SetParent(LeftHandModelGripPoint.transform);
            }
            interactor.Hand.HandAnim.SetBool(HandAnim.ToString(), true);
        }
        else
        {
            Hand = interactor.GetComponentInChildren<HandPrecence>();
            Hand.SpawnedHandModel.SetActive(false);
        }
    }
    public void RemoveHandModel(StuGrabber interactor)
    {
        if (RightHandModelGripPoint != null && LeftHandModelGripPoint != null)
        {
            interactor.ResetHand();
            interactor.Hand.HandAnim.SetBool(HandAnim.ToString(), false);
        }
        else
        {
            Hand.SpawnedHandModel.SetActive(true);
        }        
    }
    protected virtual void OnActivate()
    {
    }
    protected virtual void OnDeactivate()
    {
    }
    protected virtual void OnSecondaryButtonActivate()
    {
    }
    protected virtual void OnSecondaryButtonDeactivate()
    {
    }
    protected virtual void ThrowObject(StuGrabber interactor)
    {
        RB.AddForce(velocity * 100);
        //RB.angularVelocity = AngleVelocity;
    }
    public void ThrownByObject(Vector3 velocity)
    {
        RB.AddForce(velocity * 100);
        //RB.angularVelocity = AngleVelocity;
    }
    public void InHolster()
    {
        if (RB != null)
        {
            RB.useGravity = false;
            RB.isKinematic = true;
        }
            
    }
    public void OutOfHolster()
    {
        if (RB != null)
        {
            RB.useGravity = true;
        }
            
    }
}
    
