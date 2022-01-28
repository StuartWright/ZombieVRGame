using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StuSingleBulletGun : BaseStuGun
{
    public int Ammo, MaxAmmo;
    public override void AddMagazine(StuBaseGrabbable interactable)
    {
        //CurrentMagazine = interactable.GetComponent<StuMagazine>();
        AS.PlayOneShot(ReloadClip);
        HasSlide = false;
        Ammo++;
        //interactable.RemoveHandModel(interactable.CurrentInteractor);
        Destroy(interactable.gameObject);
    }
    public override void FireFunction()
    {
        if (OneInChamber)
        {
            Ammo--;
            NextFire = Time.time + FireRate;
            OneInChamber = false;
            if (ShootAnim != null)
                ShootAnim.SetTrigger("Fire");
            if (BulletCase != null && ShellSpawn != null)
                CasingRelease(BulletCase);
            AS.PlayOneShot(FireSound);
            foreach (ParticleSystem flash in FlashParticles)
                flash.Play();
            /*
            RaycastHit hit;
            if (Physics.Raycast(FirePoint.position, FirePoint.forward, out hit, Mathf.Infinity))
            {
                if (hit.collider.GetComponent<IDamagable>() != null)
                    hit.collider.GetComponent<IDamagable>().TakeDamage(Damage);
                GameManager.Instance.HandleRayHit(hit);
            }
            */
            Bullet bullet = Instantiate(Projectile, FirePoint.position, FirePoint.transform.rotation).GetComponent<Bullet>();
            bullet.Damage = Damage;
            bullet.Player = CurrentInteractor.Player;
            if (Recoil != null)
            {
                Recoil.AddRecoil(RecoilRotation);
                Recoil.AddPushBack(RecoilPushBack);
            }
            if (ShootAnim != null)
                ShootAnim.ResetTrigger("Fire");
            NoAmmoEvent();
        }
        else
        {
            AS.PlayOneShot(NoBulletsClip);
            NextFire = Time.time + FireRate;
        }
    }

    public override bool Slide()
    {
        if (Ammo > 0 && !OneInChamber)
        {
            HasSlide = true;
            OneInChamber = true;
            //Ammo--;
            AS.PlayOneShot(ReloadClip);
            return true;
        }
        else
        {
            return false;
        }
    }
}
