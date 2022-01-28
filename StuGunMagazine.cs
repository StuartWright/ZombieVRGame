using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class StuGunMagazine : StuBaseSocket
{
    private BaseStuGun weapon;
    public Animator Anim;
    private bool TempSocketDeactivate;
    protected SphereCollider Col;
    private void Start()
    {
        weapon = GetComponentInParent<BaseStuGun>();
        Col = GetComponent<SphereCollider>();
    }
    protected override void SocketCheck(Collider other)
    {
        //print(other.gameObject);
        if(other.tag == tag && !TempSocketDeactivate)
        {
            SelectedInteractor = other.GetComponent<StuBaseGrabbable>();
            OnSelectEnter(SelectedInteractor);
            if(SelectedInteractor != null && SelectedInteractor.CurrentInteractor != null)
            SelectedInteractor.RemoveHandModel(SelectedInteractor.CurrentInteractor);
            if (Anim != null)
            {
                Anim.gameObject.SetActive(true);
                Anim.SetTrigger("Play");
            }           
        }
    }

    public override void OnSelectEnter(StuBaseGrabbable interactor)
    {
        interactor.transform.SetParent(transform);
        interactor.gameObject.SetActive(false);
        base.OnSelectEnter(interactor);
    }
    public override void OnSelectExit(StuBaseGrabbable interactor)
    {
        if (Anim != null)
            Anim.gameObject.SetActive(false);
        interactor.gameObject.SetActive(true);
        interactor.transform.SetParent(null);
        interactor.transform.position = transform.TransformPoint(Col.center);
        interactor.ThrownByObject(weapon.velocity);
        StartCoroutine(MagCooldown());
        base.OnSelectExit(interactor);
    }

    public void EjectMag()
    {
        TempSocketDeactivate = true;
        OnSelectExit(SelectedInteractor);
    }
    protected override void CheckExit(Collider other)
    {
        if(other.tag == tag)
        {
            TempSocketDeactivate = false;
        }
    }
    IEnumerator MagCooldown()
    {
        yield return new WaitForSeconds(0.2f);
        TempSocketDeactivate = false;
    }
    public override void ForceSelect(StuBaseGrabbable interactor)
    {
        SelectedInteractor = interactor;
        OnSelectEnter(SelectedInteractor);
        Anim.gameObject.SetActive(true);
        Anim.SetTrigger("Play");
        base.ForceSelect(interactor);
    }
}
