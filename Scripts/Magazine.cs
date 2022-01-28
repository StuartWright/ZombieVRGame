using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class Magazine : XRGrabInteractable
{
    public GameObject BulletGO;
    [SerializeField]
    private int bullets;
    public int Bullets
    {
        get { return bullets; }
        set
        {
            bullets = value;
            if (Bullets == 0)
                BulletGO.SetActive(false);
        }
    }
    /*
    private void OnTriggerEnter(Collider other)
    {        
        if (other.tag == "MagDrop")
        {
            OnSelectExiting(selectingInteractor);
            OnSelectExited(selectingInteractor);
            Drop();
            other.GetComponent<GunMagazine>().AddMag(this);
        }
    }
    */
}
