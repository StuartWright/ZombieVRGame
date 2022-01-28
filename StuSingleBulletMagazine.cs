using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuSingleBulletMagazine : StuGunMagazine
{
    private StuSingleBulletGun Weapon;
    private void Start()
    {
        Weapon = GetComponentInParent<StuSingleBulletGun>();
        Col = GetComponent<SphereCollider>();
    }
    protected override void SocketCheck(Collider other)
    {
        if (other.tag == tag && Weapon.Ammo < Weapon.MaxAmmo)
        {
            SelectedInteractor = other.GetComponent<StuBaseGrabbable>();
            //OnSelectEnter(SelectedInteractor);
            Weapon.AddMagazine(SelectedInteractor);
            if (SelectedInteractor != null && SelectedInteractor.CurrentInteractor != null)
                SelectedInteractor.RemoveHandModel(SelectedInteractor.CurrentInteractor);
            SelectedInteractor = null;
            if (Anim != null)
            {
                Anim.gameObject.SetActive(false);
                Anim.gameObject.SetActive(true);
                Anim.SetTrigger("Play");
            }
        }
    }
}
