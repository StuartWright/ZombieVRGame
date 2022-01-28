using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class StuBaseSocket : MonoBehaviour
{
    public delegate void MagazineEvents(StuBaseGrabbable interactor);
    public event MagazineEvents OnSelectedEnterAction, OnSelectedExitAction;
    //public UnityEvent<StuBaseGrabbable> OnSelectedEnterAction, OnSelectedExitAction;
    public StuBaseGrabbable SelectedInteractor;
    public Transform AttachPoint;
    protected bool HasObject;
    /*
    private void OnTriggerStay(Collider other)
    {
        if (HasObject) return;
        if(SelectedInteractor == null && other != null)
        {
            if(other.GetComponent<StuBaseGrabbable>())
            {
                SelectedInteractor = other.GetComponent<StuBaseGrabbable>();
                OnSelectEnter(SelectedInteractor);
                HasObject = true;
            }
        }
        else
        {
            if (!SelectedInteractor.IsGrabbed)
            {
                if(AttachPoint != null)
                {
                    SelectedInteractor.transform.position = AttachPoint.transform.position;
                    SelectedInteractor.transform.localRotation = AttachPoint.transform.localRotation;
                }              
                else
                {
                    SelectedInteractor.transform.position = transform.position;
                    SelectedInteractor.transform.localRotation = transform.localRotation;
                }                    
            }
                
            //SelectedInteractor.transform.position = Vector3.Lerp(SelectedInteractor.transform.position, transform.position, Time.deltaTime);
        }
    }
    */
    private void OnTriggerEnter(Collider other)
    {
        if (SelectedInteractor == null && other != null)
        {
            SocketCheck(other);
        }
    }
    protected virtual void SocketCheck(Collider other)
    {

        /*
        if (other.GetComponent<StuBaseGrabbable>())
        {
            SelectedInteractor = other.GetComponent<StuBaseGrabbable>();
            OnSelectEnter(SelectedInteractor);

            if (AttachPoint != null)
            {
                SelectedInteractor.transform.position = AttachPoint.transform.position;
                SelectedInteractor.transform.localRotation = AttachPoint.transform.localRotation;
            }
            else
            {
                SelectedInteractor.transform.position = transform.position;
                SelectedInteractor.transform.localRotation = transform.localRotation;
            }
        }
        */
    }
    private void OnTriggerExit(Collider other)
    {
        CheckExit(other);
    }

    protected virtual void CheckExit(Collider other)
    {
        if (SelectedInteractor != null)
        {
            //SelectedInteractor.transform.SetParent(null);            
            OnSelectExit(SelectedInteractor);
            HasObject = false;
        }
    }
    public virtual void OnSelectEnter(StuBaseGrabbable interactor)
    {
        OnSelectedEnterAction?.Invoke(interactor);
        //interactor.transform.SetParent(transform);
        //interactor.GetComponent<Rigidbody>().useGravity = false;
    }
    public virtual void OnSelectExit(StuBaseGrabbable interactor)
    {
        OnSelectedExitAction?.Invoke(interactor);
        //interactor.transform.SetParent(null);
        //interactor.GetComponent<Rigidbody>().useGravity = true;
        SelectedInteractor = null;
    }
    public virtual void ForceSelect(StuBaseGrabbable interactor)
    { 
    }
}
