using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class StuGrabber : MonoBehaviour
{
    public InputDevice TargetDevice;
    public StuBaseGrabbable GrabbedObject;
    public List<Collider> Grabbables = new List<Collider>();
    public HandPrecence Hand;
    [HideInInspector]
    public Rigidbody RB;
    public Transform VrRig;
    //private SphereCollider Col;
    [HideInInspector]
    public Player Player;
    private Transform rig;
    public GameObject HandCanvas;
    private void Start()
    {
        RB = GetComponent<Rigidbody>();
        VrRig = GameObject.Find("VrCamera").transform;
        //Col = GetComponent<SphereCollider>();
    }
    public void SetUp(HandPrecence hand)
    {
        TargetDevice = hand.TargetDevice;
        Hand = hand;
        GameManager.Instance.Hands.Add(this);
        Player = GetComponentInParent<Player>();
        rig = Player.transform;
    }
    public void ResetHand()
    {
        Hand.ResetHandPos();
    }
    private bool HasPressed;    
    private void Update()
    {
            
        if (TargetDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool GripPressed) && GripPressed)
        {
            if(!HasPressed)
            {
                SelectObject(InteractionButtons.Grip);
                //print("grip");
            }            
        }
        else if (TargetDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool Triggerpressed) && Triggerpressed)
        {
            if(!HasPressed)
            {
                SelectObject(InteractionButtons.Trigger);
                //print("Trigger");
            }
            
        }
        else if (TargetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool AButtonpressed) && AButtonpressed)
        {
            if (!HasPressed)
            {
                SelectObject(InteractionButtons.AButton);
                //print("AButton");
            }
        }
        else if (HasPressed && GrabbedObject != null)
        {
            GrabbedObject.OnSelectExit(this);
            GrabbedObject = null;
            HasPressed = false;
        }
        else if (HasPressed)
            HasPressed = false;
        /*
        if (GrabbedObject != null)
        {
            if (TargetDevice.TryGetFeatureValue(CommonUsages.grip, out float GripValue) && GripValue < .1f)
            {
                GrabbedObject.OnSelectExit(this);
                GrabbedObject = null;
            }
        }
        */
        if (HandCanvas != null)
            CheckPalmUp();
    }

    private void CheckPalmUp()
    {
        print(VrRig.InverseTransformPoint(transform.position).z);
        //if (transform.eulerAngles.z > 50 && transform.eulerAngles.z < 120)
        if (rig.InverseTransformPoint(transform.position).z > 50 && rig.InverseTransformPoint(transform.position).z < 120)
        {
            HandCanvas.SetActive(true);
        }
        else
        {
            HandCanvas.SetActive(false);
        }
    }
    private void SelectObject(InteractionButtons button)
    {
        HasPressed = true;
        Grabbables.Clear();
        Collider nearestCollider = null;
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.2f);
        foreach(Collider col in colliders)
        {
            if (col.TryGetComponent(out StuBaseGrabbable grab))
            {
                StuBaseGrabbable obj = grab;
                if(obj.ButtonToInteract == button)
                Grabbables.Add(col);
            }                
        }
        float minSqrDistance = Mathf.Infinity;
        foreach (Collider col in Grabbables)
        {
            Vector3 directionToTarget = col.transform.position - transform.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < minSqrDistance)
            {
                minSqrDistance = dSqrToTarget;
                nearestCollider = col;
            }
        }
        if(nearestCollider != null)
        {
            if (!nearestCollider.GetComponent<StuBaseGrabbable>().IsGrabbed)
            {
                GrabbedObject = nearestCollider.GetComponent<StuBaseGrabbable>();
                GrabbedObject.OnSelectEnter(this);
            }                
        }       
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<StuBaseGrabbable>())
        {
            if(!Grabbables.Contains(other))
            {
                Grabbables.Add(other);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (Grabbables.Contains(other))
        {
            Grabbables.Remove(other);
        }
    }
    */
    /*
    private void OnTriggerStay(Collider other)
    {
        if(GrabbedObject == null)
        {
            if (TargetDevice.TryGetFeatureValue(CommonUsages.grip, out float GripValue) && GripValue > .1f && other.GetComponent<StuBaseGrabbable>())
            {
                GrabbedObject = other.GetComponent<StuBaseGrabbable>();
                GrabbedObject.OnSelectEnter(this);
            }
        }        
    }
    */

    
}
