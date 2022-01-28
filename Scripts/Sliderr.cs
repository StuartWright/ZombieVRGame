using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class Sliderr : XRGrabInteractable
{
    public Transform Gun, childPoint;
    private TargetReach SlideTarget;
    private Vector3 initialAttachLocalPos;
    private Quaternion initialAttachLocalRot;
    private void Start()
    {
        SlideTarget = GetComponent<TargetReach>();
        //Create attach point
        if (!attachTransform)
        {
            GameObject grab = new GameObject("Grab Pivot");
            grab.transform.SetParent(transform, false);
            attachTransform = grab.transform;
        }

        initialAttachLocalPos = attachTransform.localPosition;
        initialAttachLocalRot = attachTransform.localRotation;
    }

    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        SlideTarget.enabled = true;
        transform.SetParent(Gun);
        if (interactor is XRDirectInteractor)
        {
            attachTransform.position = interactor.transform.position;
            attachTransform.rotation = interactor.transform.rotation;
        }
        else
        {
            attachTransform.localPosition = initialAttachLocalPos;
            attachTransform.localRotation = initialAttachLocalRot;
        }
        base.OnSelectEntered(interactor);
    }

    protected override void OnSelectExited(XRBaseInteractor interactor)
    {
        SlideTarget.enabled = false;
        transform.SetParent(childPoint);
        base.OnSelectExited(interactor);
    }
    public void SetEverything()
    {
        interactionLayerMask = LayerMask.GetMask("Default");
    }
    public void SetNothing()
    {
        interactionLayerMask = LayerMask.GetMask("Nothing");
    }
}
